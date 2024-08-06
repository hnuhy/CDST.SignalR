using CDST.SignalR.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace CDST.SignalR.Web.Pages;

public abstract class SignalRPageModel : AbpPageModel
{
    protected SignalRPageModel()
    {
        LocalizationResourceType = typeof(SignalRResource);
    }
}
