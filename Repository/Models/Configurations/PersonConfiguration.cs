using FirstAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.Email).IsRequired();
            builder.HasMany(x => x.Tasks).WithOne(x => x.Person).HasForeignKey(x => x.PersonID);
            builder.ToTable("Persons");
        }
    }
}
