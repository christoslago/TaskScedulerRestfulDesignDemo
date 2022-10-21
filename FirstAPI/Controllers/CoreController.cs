using DataRepository.Models;
using Logic.DTOs;
using Logic.Services;
using Logic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Sample.API.Controllers
{
    public class CoreController<ObjModel> : ControllerBase where ObjModel : CoreModel
    {
        private ICoreInterface<ObjModel> Service;
        public CoreController(ICoreInterface<ObjModel> service)
        {
            Service = service;
        }
        [RequiredScope("data.write")]
        [HttpPost("Add")]
        public IActionResult Add(ObjModel obj)
        {
            var response = Service.Add(obj);
            if (response.Logger.HasErrors)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [RequiredScope("data.read")]
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var response = Service.GetAll();
            if (response.Logger.HasErrors)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [RequiredScope("data.read")]
        [HttpGet("GetByName/{name}")]
        public IActionResult GetByName(string name)
        {
            var response = Service.GetByName(name);
            if (response.Logger.HasErrors)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [RequiredScope("data.read")]
        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(Guid id)
        {
            var response = Service.GetByID(id);
            if (response.Logger.HasErrors)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [RequiredScope("data.write")]
        [HttpPatch("Update")]
        public IActionResult Update(ObjModel obj)
        {
            var response = Service.Update(obj);
            if (response.Logger.HasErrors)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [RequiredScope("data.write")]
        [HttpDelete("DeleteByID/{id}")]
        public IActionResult DeleteByID(Guid id)
        {
            var response = Service.DeleteByID(id);
            if (response.Logger.HasErrors)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
