using Volo.Abp.Modularity;

namespace CDST.SignalR;

[DependsOn(
    typeof(SignalRDomainModule),
    typeof(SignalRTestBaseModule)
)]
public class SignalRDomainTestModule : AbpModule
{

}
