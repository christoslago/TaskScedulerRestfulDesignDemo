using DataRepository.Enums;

namespace Logic.DTOs
{
    public class TaskDTO
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public  Guid PersonID { get; set; }
        public DateTime DateCreated { get; set; }
        public TaskPriorityEnum Priority { get; set; }
        public DateTime ExpiryDate { get; set; }
        public TaskStateEnum State { get; set; }
        public static TaskDTO ConvertToDTO(DataRepository.Models.Task obj)
        {
            return new TaskDTO
            {
                ID = obj.ID,
                Name = obj.Name,
                Description = obj.Description,
                PersonID = obj.PersonID,
                DateCreated = obj.DateCreated,
                Priority = obj.Priority,
                ExpiryDate = obj.ExpiryDate
            };
        }
        public static DataRepository.Models.Task ConvertFromDTO(TaskDTO dto)
        {
            return new DataRepository.Models.Task
            {
                ID = dto.ID,
                Name = dto.Name,
                Description = dto.Description,
                PersonID = dto.PersonID,
                DateCreated = dto.DateCreated,
                Priority = dto.Priority,
                ExpiryDate = dto.ExpiryDate
            };
        }
    }
}
