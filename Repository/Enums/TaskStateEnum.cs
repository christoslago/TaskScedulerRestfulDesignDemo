using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepository.Enums
{
    public enum TaskStateEnum:ushort
    {
        Assigned = 0,
        Started = 1,
        Finished = 2
    }
}
