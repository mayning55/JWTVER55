using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClassLibrary;

public class EFCoreDBContextFactory:IDesignTimeDbContextFactory<EFCoreDBContext>
{
    //仅迁移用，实际应用过程不需要。
    public EFCoreDBContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EFCoreDBContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=JWTVer;User Id=sa;Password=??Password??;TrustServerCertificate=True;");
        return new EFCoreDBContext(optionsBuilder.Options);
    }
}
