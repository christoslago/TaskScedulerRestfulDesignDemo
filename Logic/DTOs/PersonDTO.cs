using FirstAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.DTOs
{
    public class PersonDTO
    {
        public Guid ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public uint Age { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public List<TaskDTO> Tasks { get; set; }
        public PersonDTO()
        {
            Tasks = new List<TaskDTO>();
        }
        public static PersonDTO ConvertToDTO(Person obj)
        {
            var dto = new PersonDTO
            {
                ID = obj.ID,
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                Age = obj.Age,
                Height = obj.Height,
                Weight = obj.Weight
            };
            foreach(var task in obj.Tasks)
            {
                dto.Tasks.Add(TaskDTO.ConvertToDTO(task));
            }
            return dto;            
        }
        public static Person ConvertFromDTO(PersonDTO dto)
        {
            var obj = new Person
            {
                ID = dto.ID,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Age = dto.Age,
                Height = dto.Height,
                Weight = dto.Weight
            };
            foreach(var task in dto.Tasks)
            {
                obj.Tasks.Add(TaskDTO.ConvertFromDTO(task));
            }
            return obj;
        }
    }    
}
