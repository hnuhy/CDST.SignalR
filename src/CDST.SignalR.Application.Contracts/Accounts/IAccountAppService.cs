using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace CDST.SignalR.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        /// <summary>
        /// 确保Lims用户在系统中有数据，系统中没有时会进行同步初始化
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<IdentityUserDto> EnsureLimsUserInSystemAsync(string userName, string name, string password);
    }
}
