using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace CDST.SignalR.Jobs
{
    [Authorize]
    public class JobTaskAppService : ApplicationService, IJobTaskAppService
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IHubContext<JobTaskHub> _taskHubContext;
        private readonly IDistributedCache<DoingTaskDto> _distributedUserCache;

        public JobTaskAppService(IUnitOfWorkManager unitOfWorkManager, IHubContext<JobTaskHub> taskHubContext, IDistributedCache<DoingTaskDto> distributedUserCache)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _taskHubContext = taskHubContext;
            _distributedUserCache = distributedUserCache;
        }

        public async Task<JobTaskItemDto> UpdateTaskItemAsync(Guid id, CreateUpdateJobTaskItemDto input)
        {
            var jobTaskId = Guid.Empty;
            var dto = ObjectMapper.Map<CreateUpdateJobTaskItemDto, JobTaskItemDto>(input);
            return dto;
        }

        public async Task<JobTaskItemDetailDto> CreateTaskItemDetailAsync(CreateUpdateJobTaskItemDetailDto input)
        {
           
            var dto = ObjectMapper.Map<CreateUpdateJobTaskItemDetailDto, JobTaskItemDetailDto>(input);

            return dto;
        }

        public async Task<JobTaskItemDetailDto> SaveTaskItemDetailAsync(CreateUpdateJobTaskItemDetailDto input)
        {
            var dto = new JobTaskItemDetailDto();

            using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
            {
                if (input.Id != null && input.Id.HasValue && !input.Id.Value.Equals(Guid.Empty))
                {
                    dto = await UpdateTaskItemDetailAsync(input.Id.Value, input);
                }
                else
                {
                    dto = await CreateTaskItemDetailAsync(input);
                }

                await uow.CompleteAsync();
            }

            var taskId = Guid.Empty;
            if (input.JobTaskId.HasValue)
            {
                taskId = input.JobTaskId.Value;
            }

            //可能会新增或是删除了一行数据，这个时候要通知前端刷新数量的更新
            await NotifyTaskCountsUpdatedAsync(taskId);

            var resultDto = new JobTaskItemDto();

            //同时，还要刷新这个任务项的内容
            //await NotifyTaskItemUpdated(taskId, dto.JobTaskItemId.Value, resultDto);

            return dto;
        }

        public async Task<JobTaskItemDetailDto> UpdateTaskItemDetailAsync(Guid id, CreateUpdateJobTaskItemDetailDto input)
        {
            var jobTaskId = Guid.Empty;
            var dto = ObjectMapper.Map<CreateUpdateJobTaskItemDetailDto, JobTaskItemDetailDto>(input);

            return dto;
        }

        /// <summary>
        /// 通知任务的数量有变化
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private async Task NotifyTaskCountsUpdatedAsync(Guid taskId)
        {
            var userCacheValue = await _distributedUserCache.GetAsync(CurrentUser.Id.Value.ToString());

            if (userCacheValue != null && !string.IsNullOrWhiteSpace(userCacheValue.ConnectionId))
            { 
                var curConnectionId = userCacheValue.ConnectionId;

                await _taskHubContext.Clients.GroupExcept(taskId.ToString(), curConnectionId).SendAsync("UpdateStatus",
                    new
                    {
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        WithUserId = CurrentUser.Id,
                        WithUserName = CurrentUser.Name,
                        TaskId = taskId.ToString(),
                        Action = "TaskCountsUpdated",
                        Data = DateTime.Now.Ticks
                    });

                return;
            }

            await _taskHubContext.Clients.Group(taskId.ToString()).SendAsync("UpdateStatus",
                   new
                   {
                       ConcurrencyStamp = Guid.NewGuid().ToString(),
                       WithUserId = CurrentUser.Id,
                       WithUserName = CurrentUser.Name,
                       TaskId = taskId.ToString(),
                       Action = "TaskCountsUpdated",
                       Data = DateTime.Now.Ticks
                   });

            //await _taskHubContext.Clients.All.SendAsync("UpdateStatus",
            //        new
            //        {
            //            ConcurrencyStamp = Guid.NewGuid().ToString(),
            //            WithUserId = CurrentUser.Id,
            //            WithUserName = CurrentUser.Name,
            //            TaskId = taskId.ToString(),
            //            Action = "TaskCountsUpdated",
            //            Data = DateTime.Now.Ticks
            //        });
        }

        /// <summary>
        /// 通知任务项有更新（如：添加或是删除了一行）
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskItemDto"></param>
        /// <returns></returns>
        private async Task NotifyTaskItemUpdated(Guid taskId, Guid taskItemId, JobTaskItemDto taskItemDto)
        {
            //var evt = new JobTaskUpdatedNotifyEvent { WithUserId = CurrentUser.Id, WithUserName = CurrentUser.Name, TaskItemId = taskItemId, Action = "TaskItemUpdated", Data = taskItemDto };
            //await _localEventBus.PublishAsync(evt);
            //using (await _Mutex.LockAsync())
            {
                var concurrencyStamp = Guid.NewGuid().ToString();
                var model = new JobTaskItemDto();
                var sendData = new
                {
                    ConcurrencyStamp = concurrencyStamp,
                    WithUserId = CurrentUser.Id,
                    WithUserName = CurrentUser.Name,
                    TaskId = taskId.ToString(),
                    TaskItemId = taskItemId.ToString(),
                    Action = "TaskItemUpdated",
                    Data = model
                };
                await _taskHubContext.Clients.All.SendAsync("UpdateStatus", sendData);

            }

        }
    }
}
