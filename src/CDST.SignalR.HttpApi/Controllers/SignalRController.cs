using CDST.SignalR.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace CDST.SignalR.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class SignalRController : AbpControllerBase
{
    protected SignalRController()
    {
        LocalizationResource = typeof(SignalRResource);
    }
}
