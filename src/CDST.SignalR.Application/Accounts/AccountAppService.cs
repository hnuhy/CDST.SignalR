using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Identity;

namespace CDST.SignalR.Accounts
{
    [AllowAnonymous]
    public class AccountAppService : SignalRAppService, IAccountAppService
    {

        private readonly IdentityUserManager _userManager;

        public AccountAppService(IdentityUserManager userManager)
        {
            _userManager = userManager;
        }

        [RemoteService(IsEnabled = false, IsMetadataEnabled = false)]
        [AllowAnonymous]
        public async Task<IdentityUserDto> EnsureLimsUserInSystemAsync(string userName, string name, string password)
        {
            var result = new IdentityUserDto();
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new IdentityUser(GuidGenerator.Create(), userName, $"{userName}@lims.org")
                {
                    Name = name
                };

                await _userManager.CreateAsync(user, password, validatePassword: false);
            }

            result = ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);

            return result;
        }
    }
}
