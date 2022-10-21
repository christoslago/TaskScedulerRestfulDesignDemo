using DataRepository.Models;
using DataRepository.Models.Configurations;
using FirstAPI.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Models.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
        public DbSet<T> getDBSet<T>() where T : class
        {
            return Set<T>();
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        public DbSet<DataRepository.Models.Task> Tasks { get; set; }        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new PersonConfiguration());
            builder.ApplyConfiguration(new SupervisorConfiguration());
            builder.ApplyConfiguration(new TaskConfiguration());
        }
    }
}
