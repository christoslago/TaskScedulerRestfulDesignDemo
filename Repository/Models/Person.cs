using DataRepository.Models;
using FirstAPI.Enums;

namespace FirstAPI.Models
{
    public class Person:CoreUser
    {
        
        public decimal Income { get; set; }
        public string? AzurePrincipalID { get; set; }
        public List<DataRepository.Models.Task> Tasks { get; set; }
        public Person():base()
        {
            IdentityType = IdentityEnum.Visitor;
            Tasks = new List<DataRepository.Models.Task>();
        }
    }
}
