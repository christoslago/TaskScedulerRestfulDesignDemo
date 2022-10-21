using FirstAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepository.Models
{
    public class CoreUser : CoreModel
    {
        public new string? Name { get { return FirstName + " " + LastName; } }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public IdentityEnum IdentityType { get; set; }
        public uint Age { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public CoreUser():base()
        {

        }
    }
}
