using Volo.Abp.Modularity;

namespace CDST.SignalR;

/* Inherit from this class for your domain layer tests. */
public abstract class SignalRDomainTestBase<TStartupModule> : SignalRTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
