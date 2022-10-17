using FirstAPI.Models;
using Logic.DTOs;
using Logic.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services.Interfaces
{
    public interface IPersonsService
    {
        Envelope<PersonDTO> GetAllPersons();
        Envelope<PersonDTO> GetPersonById(Guid id);
        Envelope<PersonDTO> GetPersonsByName(string name);
        Envelope<PersonDTO> AddNewPerson(PersonDTO person);
        Envelope<PersonDTO> UpdatePerson(PersonDTO person);
        Envelope<PersonDTO> DeletePersonByID(Guid id);
        Envelope<PersonDTO> EditPersonTasks(PersonDTO dto);
    }
}
