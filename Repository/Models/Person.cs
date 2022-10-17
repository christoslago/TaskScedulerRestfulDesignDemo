using FirstAPI.Enums;

namespace FirstAPI.Models
{
    public class Person
    {
        public Guid ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public IdentityEnum IdentityType { get; set; }
        public DateTime DateCreated { get; set; }
        public uint Age { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public decimal Income { get; set; }
        public List<DataRepository.Models.Task> Tasks { get; set; }
        public Person()
        {
            ID = Guid.NewGuid();
            DateCreated = DateTime.UtcNow;
            IdentityType = IdentityEnum.Visitor;
            Tasks = new List<DataRepository.Models.Task>();
        }
    }
}
