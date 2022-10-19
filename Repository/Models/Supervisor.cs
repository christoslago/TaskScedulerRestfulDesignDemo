using FirstAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepository.Models
{
    public class Supervisor:CoreUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Supervisor():base()
        {
            IdentityType = FirstAPI.Enums.IdentityEnum.Admin;
        }
    }
}
