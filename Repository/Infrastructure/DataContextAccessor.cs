using Logic.Helpers;
using Microsoft.EntityFrameworkCore;
using Repository.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Infrastructure
{
    public class DataContextAccessor: IContextAccessor
    {
        public DataContext Context;        
        public DataContextAccessor()
        {
            
        }
        public void SetContext(string connection)
        {
            //Context = new DataContext();
        }
        public DbSet<T> getDBSet<T>() where T : class
        {
            return Context.Set<T>();
        }
    }
}
