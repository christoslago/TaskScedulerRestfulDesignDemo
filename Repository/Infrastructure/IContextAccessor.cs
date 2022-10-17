using Microsoft.EntityFrameworkCore;
using Repository.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Helpers
{
    public interface IContextAccessor
    {
        public void SetContext(string connection);
        public DbSet<T> getDBSet<T>() where T : class;
    }
}
