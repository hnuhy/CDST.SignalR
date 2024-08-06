using CDST.SignalR.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace CDST.SignalR.Permissions;

public class SignalRPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(SignalRPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(SignalRPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SignalRResource>(name);
    }
}
