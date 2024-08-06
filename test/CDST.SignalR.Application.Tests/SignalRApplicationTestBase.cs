using Volo.Abp.Modularity;

namespace CDST.SignalR;

public abstract class SignalRApplicationTestBase<TStartupModule> : SignalRTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
