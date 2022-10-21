using DataRepository.Models;
using Logic.Services.Interfaces;
using Repository.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public class SupervisorsService:CoreService<Supervisor>,ISupervisorsService
    {
        private DataContext ContextAccessor;
        public SupervisorsService(DataContext contextAccessor):base(contextAccessor)
        {
            ContextAccessor = contextAccessor;
        }
    }
}
