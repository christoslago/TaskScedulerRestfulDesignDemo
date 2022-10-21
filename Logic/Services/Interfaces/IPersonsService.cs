using DataRepository.Models;
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
    public interface IPersonsService:ICoreInterface<Person>
    {
        Envelope<PersonDTO> EditPersonTasks(PersonDTO dto,string assignerName);
        Envelope<PersonDTO> GetPersonWithTasksDTO(string Upn);
        Task<Envelope<AzureUserDTO>> GetAzureUsers();
        Task<Envelope<PersonDTO>> SavePersonsFromAzureUsers();
    }
}
