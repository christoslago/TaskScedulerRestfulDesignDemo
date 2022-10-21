using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepository.Models
{
    public class CoreModel
    {
        public Guid ID { get; set; }
        public string? Name { get; set; }
        public DateTime DateCreated { get; set; }
        public CoreModel()
        {
            ID = Guid.NewGuid();
            DateCreated = DateTime.UtcNow;
        }
    }
}
