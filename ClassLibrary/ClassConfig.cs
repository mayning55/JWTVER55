using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityDemo.Models;

public class ClassConfig
{
    public class PersonConfig : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Persons");
        }
    }

    public class GunDamConfig : IEntityTypeConfiguration<GunDam>
    {
        public void Configure(EntityTypeBuilder<GunDam> builder)
        {
            builder.ToTable("GunDams");
        }
    }
}
