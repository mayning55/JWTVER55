using System;
using IdentityDemo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary;

public class EFCoreDBContext:IdentityDbContext<UserExtend>
{
    public EFCoreDBContext(DbContextOptions<EFCoreDBContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<GunDam> GunDams { get; set; }

}
