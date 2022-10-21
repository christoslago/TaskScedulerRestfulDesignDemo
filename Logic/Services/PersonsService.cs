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
using Microsoft.Identity.Client;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.Graph;
using Person = FirstAPI.Models.Person;

namespace Logic.Services
{
    public class PersonsService :CoreService<Person>, IPersonsService
    {
        private DataContext ContextAccessor;
        private IEmailsService MailService;
        private IAzureService AzureService;
        public PersonsService(DataContext contextAccessor, IEmailsService emailService,IAzureService azureService) : base(contextAccessor)
        {
            ContextAccessor = contextAccessor;
            MailService = emailService;
            AzureService = azureService;
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
            //var azureResp = CreatePersonInAzure(obj);
            //if (!azureResp)
            //{
            //    coll.Logger.AddError("Problem While saving to Active Directory", "CreateNewUser");
            //}
            return coll;
        }       
        public Envelope<PersonDTO> GetPersonWithTasksDTO(string upn)
        {
            var collection = new Envelope<PersonDTO>();
            var obj = ContextAccessor.getDBSet<Person>().Where(x => x.AzurePrincipalID == upn).Include(x => x.Tasks).AsNoTracking().FirstOrDefault();
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
            var tasksToAlert = new List<Guid>();
            foreach (var taskObj in objInDB.Tasks)
            {
                
                if (dto.Tasks.Select(x => x.ID).Contains(taskObj.ID))
                {
                    var taskDTO = dto.Tasks.Where(x => x.ID == taskObj.ID).FirstOrDefault();
                    if (taskDTO.ExpiryDate != taskObj.ExpiryDate || taskDTO.Description != taskObj.Description || taskDTO.Name != taskObj.Name || taskDTO.Priority != taskObj.Priority)
                    {
                        taskDTO.State = DataRepository.Enums.TaskStateEnum.Updated;
                        ContextAccessor.Entry(taskObj).CurrentValues.SetValues(taskDTO);
                        ContextAccessor.Entry(taskObj).State = EntityState.Modified;
                        tasksToAlert.Add(taskObj.ID);
                    }
                   
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
                    taskObj.State = DataRepository.Enums.TaskStateEnum.Assigned;
                    taskObj.DateCreated = DateTime.UtcNow;
                    taskObj.PersonID = objInDB.ID;
                    taskObj.Person = objInDB;
                    taskObj.Description += "\n" + " Assigned by: " + assignerName;
                    objInDB.Tasks.Add(taskObj);
                    ContextAccessor.getDBSet<DataRepository.Models.Task>().Add(taskObj);
                    ContextAccessor.Entry(taskObj).State = EntityState.Added;
                    tasksToAlert.Add(taskObj.ID);
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
                var returnObj = ContextAccessor.getDBSet<FirstAPI.Models.Person>().Include(x => x.Tasks).Where(x => x.ID == dto.ID).FirstOrDefault();
                foreach(var id in tasksToAlert)
                {
                    var res = MailService.SendTaskWithEmail(returnObj.Tasks.Where(x => x.ID == id).FirstOrDefault(), objInDB.Email, assignerName);
                    if (res)
                    {
                        collection.Collection.Add(PersonDTO.ConvertToDTO(returnObj));
                    }
                    else
                    {
                        collection.Logger.AddError("Problem while sending the email", "Person.UpdateTasks");
                    }
                }                
            }
            return collection;
        }

        public Envelope<AzureUserDTO> GetAzureUsers()
        {
            var collection = new Envelope<AzureUserDTO>();
            var azureUsers = AzureService.GetAzureUsers();

            foreach(var usr in azureUsers.Result)
            {
                var newUser = new AzureUserDTO();
                newUser.ID = Guid.Parse(usr.Id);
                newUser.FullName = usr.DisplayName;
                newUser.FirstName = usr.GivenName;
                newUser.LastName = usr.Surname;
                var ids = usr.Identities.ToList();
                var mail = ids[0].IssuerAssignedId;
                newUser.Email = mail;
                collection.Collection.Add(newUser);
            }
            if(azureUsers.Result.Count == 0)
            {
                collection.Logger.AddError("No azure users found","GetAzureUsers");
            }
            return collection;
        }
        public Envelope<PersonDTO> SavePersonsFromAzureUsers()
        {
            var azureUsers = this.GetAzureUsers();
            var collection = new Envelope<PersonDTO>();
            if (azureUsers.Logger.HasErrors)
            {
                collection.Logger.AddError("Problem while fetching azure users", "GetAzureUsers");
            }
            else
            {
                foreach(var azrUser in azureUsers.Collection)
                {
                    var result = Guid.NewGuid();
                    var foundInDB = ContextAccessor.getDBSet<Person>().Where(x => x.AzurePrincipalID == azrUser.ID.ToString()).AsNoTracking().FirstOrDefault();
                    if(foundInDB == null)
                    {
                        var personObj = new Person {ID = Guid.NewGuid(), AzurePrincipalID = azrUser.ID.ToString(),DateCreated = DateTime.UtcNow,FirstName = azrUser.FirstName,LastName = azrUser.LastName,Email=azrUser.Email};
                        var addResponse = this.Add(personObj);
                        if (addResponse.Logger.HasErrors)
                        {
                            collection.Logger.AddError("Problem while saving the entity to db", "SaveUserToLocalDB");
                        }
                        else
                        {
                            collection.Collection.Add(PersonDTO.ConvertToDTO(personObj));
                        }
                    }
                }
            }
            return collection;
        }
    }
}
