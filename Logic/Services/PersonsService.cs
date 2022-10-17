using DataRepository.Models;
using FirstAPI.Models;
using Logic.DTOs;
using Logic.Helpers;
using Logic.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repository.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public class PersonsService : IPersonsService
    {
        private DataContext ContextAccessor;
        public PersonsService(DataContext contextAccessor)
        {
            ContextAccessor = contextAccessor;
        }
        public Envelope<PersonDTO> AddNewPerson(PersonDTO dto)
        {
            var collection = new Envelope<PersonDTO>();
            var obj = PersonDTO.ConvertFromDTO(dto);
            ContextAccessor.getDBSet<Person>().Add(obj);
            ContextAccessor.Entry(obj).State = EntityState.Added;
            var savedResult = 0;
            try
            {
                savedResult = ContextAccessor.SaveChanges();
            }catch(Exception e)
            {
                collection.Logger.AddError(e.Message, "Persons.AddNewPerson");
            }
            
            if(savedResult > 0)
            {
                collection.Collection.Add(PersonDTO.ConvertToDTO(obj));
            }
            else
            {
                collection.Logger.AddError("Error while writing Database","Persons.AddNewPerson");
            }
            return collection;
        }

       
        public Envelope<PersonDTO> EditPersonTasks(PersonDTO dto)
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
                collection.Collection.Add(PersonDTO.ConvertToDTO(returnObj));
            }
            return collection;
        }
        public Envelope<PersonDTO> DeletePersonByID(Guid personID)
        {
            var collection = new Envelope<PersonDTO>();
            var obj = ContextAccessor.getDBSet<Person>().Where(x => x.ID == personID).FirstOrDefault();
            if(obj == null)
            {
                collection.Logger.AddError("Person not found to delete", "Persons.DeletePersonByID");
                return collection;
            }
            var savedResult = 0;
            ContextAccessor.getDBSet<Person>().Remove(obj);
            ContextAccessor.Entry(obj).State = EntityState.Deleted;
            try
            {
                savedResult = ContextAccessor.SaveChanges();
            }catch(Exception e)
            {
                collection.Logger.AddError(e.Message, "Persons.DeletePersonByID");
            }
            return collection;
        }

        public Envelope<PersonDTO> GetAllPersons()
        {
            var collection = new Envelope<PersonDTO>();
            var objects = ContextAccessor.getDBSet<Person>().AsNoTracking().Include(x => x.Tasks).ToList();
            if(objects.Count == 0)
            {
                collection.Logger.AddError("No Persons Found", "Persons.GetAllPersons");
            }
            else
            {
                foreach(var obj in objects)
                {
                    collection.Collection.Add(PersonDTO.ConvertToDTO(obj));
                }
            }
            return collection;
        }

        public Envelope<PersonDTO> GetPersonById(Guid personID)
        {
            var collection = new Envelope<PersonDTO>();
            var obj = ContextAccessor.getDBSet<Person>().AsNoTracking().Where(x => x.ID == personID).Include(x => x.Tasks).FirstOrDefault();
            if(obj != null)
            {
                collection.Collection.Add(PersonDTO.ConvertToDTO(obj));
            }
            else
            {
                collection.Logger.AddError("Person not found", "Persons.GetPersonById");
            }
            return collection;
        }

        public Envelope<PersonDTO> GetPersonsByName(string personName)
        {
            var collection = new Envelope<PersonDTO>();
            var objects = ContextAccessor.getDBSet<Person>().AsNoTracking().Where(x => x.FirstName.ToLower() == personName.ToLower()).Include(x => x.Tasks).ToList();
            if (objects.Count > 0)
            {
                foreach(var obj in objects)
                {
                    collection.Collection.Add(PersonDTO.ConvertToDTO(obj));
                }
            }
            else
            {
                collection.Logger.AddError("Person not found", "Persons.GetPersonById");
            }
            return collection;
        }

        public Envelope<PersonDTO> UpdatePerson(PersonDTO dto)
        {
            var collection = new Envelope<PersonDTO>();
            var obj = ContextAccessor.getDBSet<Person>().Where(x => x.ID == dto.ID).FirstOrDefault();
            if(obj == null)
            {
                collection.Logger.AddError("Person not found to update", "Person.UpdatePerson");
                return collection;
            }
            var savedResult = 0;
            try
            {

                ContextAccessor.Entry(obj).CurrentValues.SetValues(dto);
                savedResult = ContextAccessor.SaveChanges();
            }catch(Exception e)
            {
                collection.Logger.AddError(e.Message, "Persons.UpdatePerson");
            }
            if(savedResult > 0)
            {
                collection.Collection.Add(dto);
            }
            return collection;            
        }
    }
}
