using DataRepository.Enums;
using FirstAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepository.Models
{
    public class Task
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Person Person { get; set; }
        public Guid PersonID { get; set; }
        public DateTime DateCreated { get; set; }
        public TaskPriorityEnum Priority { get; set; }
        public DateTime ExpiryDate { get; set; }
        public TaskStateEnum State { get; set; }
        public Task()
        {
            ID = Guid.NewGuid();
            State = TaskStateEnum.Assigned;
            DateCreated = DateTime.UtcNow;
            Priority = TaskPriorityEnum.Medium;
        }
    }
}
