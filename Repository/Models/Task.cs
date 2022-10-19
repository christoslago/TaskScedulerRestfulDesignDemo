using DataRepository.Enums;
using FirstAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepository.Models
{
    public class Task:CoreModel
    {
        public string? Description { get; set; }
        public Person Person { get; set; }
        public Guid PersonID { get; set; }
        public TaskPriorityEnum Priority { get; set; }
        public DateTime ExpiryDate { get; set; }
        public TaskStateEnum State { get; set; }
        public Task():base()
        {
            State = TaskStateEnum.Assigned;
            Priority = TaskPriorityEnum.Medium;
        }
    }
}
