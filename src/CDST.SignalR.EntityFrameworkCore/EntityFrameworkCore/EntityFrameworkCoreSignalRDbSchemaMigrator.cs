using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CDST.SignalR.Data;
using Volo.Abp.DependencyInjection;

namespace CDST.SignalR.EntityFrameworkCore;

public class EntityFrameworkCoreSignalRDbSchemaMigrator
    : ISignalRDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreSignalRDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the SignalRDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<SignalRDbContext>()
            .Database
            .MigrateAsync();
    }
}
