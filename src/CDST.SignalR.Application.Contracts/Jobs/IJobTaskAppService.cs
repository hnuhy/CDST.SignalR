using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace CDST.SignalR.Jobs
{
    public interface IJobTaskAppService : IApplicationService
    {
        /// <summary>
        /// 更新执行任务项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<JobTaskItemDto> UpdateTaskItemAsync(Guid id, CreateUpdateJobTaskItemDto input);

        /// <summary>
        /// 添加执行任务项数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<JobTaskItemDetailDto> CreateTaskItemDetailAsync(CreateUpdateJobTaskItemDetailDto input);

        /// <summary>
        /// 更新执行任务项数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<JobTaskItemDetailDto> UpdateTaskItemDetailAsync(Guid id, CreateUpdateJobTaskItemDetailDto input);

        /// <summary>
        /// 保存执行任务项数据（有则更新，没有则添加）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<JobTaskItemDetailDto> SaveTaskItemDetailAsync(CreateUpdateJobTaskItemDetailDto input);

    }
}
