using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace CDST.SignalR.Data;

/* This is used if database provider does't define
 * ISignalRDbSchemaMigrator implementation.
 */
public class NullSignalRDbSchemaMigrator : ISignalRDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
