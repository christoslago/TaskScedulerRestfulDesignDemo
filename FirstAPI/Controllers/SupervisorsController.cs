using DataRepository.Models;
using Logic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sample.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SupervisorsController : CoreController<Supervisor>
    {
        private ISupervisorsService SupervisorsService;
        public SupervisorsController(ISupervisorsService supervisorsService):base(supervisorsService)
        {
            SupervisorsService = supervisorsService;
        }
    }
}
