using FirstAPI.Enums;
using FirstAPI.Models;
using Logic.DTOs;
using Logic.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.OpenApi.Extensions;
using Sample.API.Controllers;
using System.Net;

namespace FirstAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PersonsController : CoreController<Person>
    {
        private IPersonsService PersonsService;
        public PersonsController(IPersonsService personsService):base(personsService)
        {
            PersonsService = personsService;
        }
        [RequiredScope("data.write")]
        [HttpPost("EditPersonTasks")]
        public IActionResult EditPersonTasks(PersonDTO dto)
        {
            var userFromFirstName = User.Claims.Single(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname").Value;
            var userFromLastName = User.Claims.Single(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname").Value;
            var assignerName = userFromFirstName + " " + userFromLastName;

            var response = PersonsService.EditPersonTasks(dto,assignerName);
            if (response.Logger.HasErrors)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [RequiredScope("data.read")]
        [HttpGet("MyTasks")]
        public IActionResult GetMyTasks()
        {
            var response = PersonsService.GetPersonWithTasksDTO(User.Claims.Single(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value);
            if (response.Logger.HasErrors)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [RequiredScope("data.read")]
        [HttpGet("AzureUsers")]
        public IActionResult GetAzureUsers()
        {
            var resp = PersonsService.GetAzureUsers();
            if (resp.Logger.HasErrors)
            {
                return BadRequest(resp);
            }
            return Ok(resp);
        }
        [RequiredScope("data.read")]
        [HttpGet("SaveAzureUsers")]
        public IActionResult SaveAzureUsers()
        {
            var resp = PersonsService.SavePersonsFromAzureUsers();
            if (resp.Logger.HasErrors)
            {
                return BadRequest(resp);
            }
            return Ok(resp);
        }
    }
   
}