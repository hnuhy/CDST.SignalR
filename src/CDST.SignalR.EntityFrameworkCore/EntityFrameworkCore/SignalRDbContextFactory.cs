using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CDST.SignalR.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class SignalRDbContextFactory : IDesignTimeDbContextFactory<SignalRDbContext>
{
    public SignalRDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        SignalREfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<SignalRDbContext>()
            .UseSqlite(configuration.GetConnectionString("Default"));
        
        return new SignalRDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../CDST.SignalR.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
