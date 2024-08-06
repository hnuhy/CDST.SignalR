using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace CDST.SignalR.Web;

[Dependency(ReplaceServices = true)]
public class SignalRBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "SignalR";
}
