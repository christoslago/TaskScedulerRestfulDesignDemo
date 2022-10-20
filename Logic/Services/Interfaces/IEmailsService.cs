using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services.Interfaces
{
    public interface IEmailsService
    {
        bool SendTaskWithEmail(DataRepository.Models.Task taskObject, string emailTO, string TaskFrom);
    }
}
