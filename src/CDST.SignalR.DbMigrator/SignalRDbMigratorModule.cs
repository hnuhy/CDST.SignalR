using CDST.SignalR.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace CDST.SignalR.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(SignalREntityFrameworkCoreModule),
    typeof(SignalRApplicationContractsModule)
)]
public class SignalRDbMigratorModule : AbpModule
{
}
