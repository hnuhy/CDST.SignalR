using Volo.Abp.Modularity;

namespace CDST.SignalR;

[DependsOn(
    typeof(SignalRApplicationModule),
    typeof(SignalRDomainTestModule)
)]
public class SignalRApplicationTestModule : AbpModule
{

}
