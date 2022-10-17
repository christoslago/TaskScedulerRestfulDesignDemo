using FirstAPI.Enums;
using FirstAPI.Models;
using Logic.DTOs;
using Logic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace FirstAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonsController : ControllerBase
    {
        private IPersonsService PersonsService;
        public PersonsController(IPersonsService personsService)
        {
            PersonsService = personsService;
        }
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var response = PersonsService.GetAllPersons();
            if (response.Logger.HasErrors)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(Guid id)
        {
            var response = PersonsService.GetPersonById(id);
            if (response.Logger.HasErrors)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpGet("GetByName/{name}")]
        public IActionResult GetByName(string name)
        {
            var response = PersonsService.GetPersonsByName(name);
            if (response.Logger.HasErrors)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpPost("AddNew")]
        public IActionResult AddNew(PersonDTO dto)
        {
            var response = PersonsService.AddNewPerson(dto);
            if (response.Logger.HasErrors)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpPatch("Update")]
        public IActionResult UpdatePerson(PersonDTO dto)
        {
            var response = PersonsService.UpdatePerson(dto);
            if (response.Logger.HasErrors)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpGet("DeleteByID/{id}")]
        public IActionResult DeletePersonByID(Guid id)
        {
            var response = PersonsService.DeletePersonByID(id);
            if (response.Logger.HasErrors)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }       
        [HttpPost("EditPersonTasks")]
        public IActionResult EditPersonTasks(PersonDTO dto)
        {
            var response = PersonsService.EditPersonTasks(dto);
            if (response.Logger.HasErrors)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
   
}