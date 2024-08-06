using CDST.SignalR.Localization;
using Volo.Abp.Application.Services;

namespace CDST.SignalR;

/* Inherit your application services from this class.
 */
public abstract class SignalRAppService : ApplicationService
{
    protected SignalRAppService()
    {
        LocalizationResource = typeof(SignalRResource);
    }
}
