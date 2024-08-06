using System.Threading.Tasks;

namespace CDST.SignalR.Data;

public interface ISignalRDbSchemaMigrator
{
    Task MigrateAsync();
}
