using DataRepository.Models;
using FirstAPI.Models;
using Logic.DTOs;
using Logic.Helpers;
using Logic.Services.Interfaces;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit.Text;
using MimeKit;
using Repository.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace Logic.Services
{
    public class PersonsService :CoreService<Person>, IPersonsService
    {
        private DataContext ContextAccessor;
        private IEmailsService MailService;
        public PersonsService(DataContext contextAccessor, IEmailsService emailService) : base(contextAccessor)
        {
            ContextAccessor = contextAccessor;
            MailService = emailService;
            QueryIncludes();
        }
        public override void QueryIncludes(List<string> empty = null)
        {
            var lst = new List<string>();
            lst.Add("Tasks");
            base.QueryIncludes(lst);
        }

        public override Person BeforeAdd(Person obj)
        {
            var person = base.BeforeAdd(obj);
            if(person.IdentityType == null)
            {
                person.IdentityType = FirstAPI.Enums.IdentityEnum.User;
            }
            
            return person;
        }
        private bool CreatePersonInAzure(Person obj)
        {

            return true;
        }
        public override Envelope<Person> Add(Person obj)
        {
            var coll = base.Add(obj);
            var azureResp = CreatePersonInAzure(obj);
            if (!azureResp)
            {
                coll.Logger.AddError("Problem While saving to Active Directory", "CreateNewUser");
            }
            return coll;
        }
        public Guid GetMyIDByADPrincipalName(string principalName)
        {
            var objFromDB = ContextAccessor.getDBSet<Person>().Where(x => x.AzurePrincipalID == principalName).FirstOrDefault();
            if(objFromDB != null)
            {
                return objFromDB.ID;
            }
            else
            {
                return Guid.Empty;
            }
        }
        public Envelope<PersonDTO> GetPersonTasksDTO(Guid id)
        {
            var collection = new Envelope<PersonDTO>();
            var obj = ContextAccessor.getDBSet<Person>().Where(x => x.ID == id).Include(x => x.Tasks).AsNoTracking().FirstOrDefault();
            if(obj == null)
            {
                collection.Logger.AddError("Couldn't find your profile", "GetMyTasks");
            }
            else
            {
                collection.Collection.Add(PersonDTO.ConvertToDTO(obj));
            }
            return collection;
        }
        public Envelope<PersonDTO> EditPersonTasks(PersonDTO dto,string assignerName)
        {
            var collection = new Envelope<PersonDTO>();
            var objInDB = ContextAccessor.getDBSet<Person>().Include(x => x.Tasks).Where(x => x.ID == dto.ID).FirstOrDefault();
            if(objInDB == null)
            {
                collection.Logger.AddError("Person not found to remove task", "Persons.RemoveTaskFromPerson");
                return collection;
            }
            foreach (var taskObj in objInDB.Tasks)
            {
                
                if (dto.Tasks.Select(x => x.ID).Contains(taskObj.ID))
                {
                    var taskDTO = dto.Tasks.Where(x => x.ID == taskObj.ID).FirstOrDefault();
                    ContextAccessor.Entry(taskObj).CurrentValues.SetValues(taskDTO);
                    ContextAccessor.Entry(taskObj).State = EntityState.Modified;
                }
                else
                {
                    ContextAccessor.getDBSet<DataRepository.Models.Task>().Remove(taskObj);
                    ContextAccessor.Entry(taskObj).State = EntityState.Deleted;                
                }
            }
            foreach (var taskDTO in dto.Tasks)
            {
                if (!objInDB.Tasks.Select(x => x.ID).Contains(taskDTO.ID))
                {
                    var taskObj = TaskDTO.ConvertFromDTO(taskDTO);
                    taskObj.ID = Guid.NewGuid();
                    taskObj.DateCreated = DateTime.UtcNow;
                    taskObj.PersonID = objInDB.ID;
                    taskObj.Person = objInDB;
                    taskObj.Description += "\n" + "Assigned by: " + assignerName;
                    objInDB.Tasks.Add(taskObj);
                    ContextAccessor.getDBSet<DataRepository.Models.Task>().Add(taskObj);
                    ContextAccessor.Entry(taskObj).State = EntityState.Added;
                }
            }
            var savedResult = 0;
            try
            {
                savedResult = ContextAccessor.SaveChanges();
            }catch(Exception e)
            {
                collection.Logger.AddError(e.Message, "Persons.EditPersonTasks");
            }
            if( savedResult > 0)
            {
                var returnObj = ContextAccessor.getDBSet<Person>().Include(x => x.Tasks).Where(x => x.ID == dto.ID).FirstOrDefault();
                var res = MailService.SendTaskWithEmail(returnObj.Tasks.OrderBy(x => x.DateCreated).LastOrDefault(),objInDB.Email,assignerName);
                if (res)
                {
                    collection.Collection.Add(PersonDTO.ConvertToDTO(returnObj));
                }
                else
                {
                    collection.Logger.AddError("Problem while sending the email", "Person.UpdateTasks");
                }
            }
            return collection;
        }

        
    }
}
