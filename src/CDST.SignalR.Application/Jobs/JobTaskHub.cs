using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Caching;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CDST.SignalR.Jobs
{
    [HubRoute("signalr-hubs/task")]
    [Authorize]
    public class JobTaskHub : AbpHub
    {
        public const string CacheKey = nameof(JobTaskHub);

        /// <summary>
        /// 用户结构：
        /// 用户1--操作数据
        /// 用户2--操作数据
        /// </summary>
        private readonly IDistributedCache<DoingTaskDto> _distributedUserCache;

        /// <summary>
        /// 任务结构：
        /// 任务1--用户1--正在操作的数据
        ///      --用户2--正在操作的数据
        /// 任务2--用户1--正在操作的数据
        /// </summary>
        private readonly IDistributedCache<DoingTaskCacheItemDto> _distributedTaskCache;

        public JobTaskHub(IDistributedCache<DoingTaskDto> distributedUserCache, IDistributedCache<DoingTaskCacheItemDto> distributedTaskCache)
        {
            _distributedUserCache = distributedUserCache;
            _distributedTaskCache = distributedTaskCache;
        }

        /// <summary>
        /// 添加到分组
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task AddToGroup(Guid taskId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, taskId.ToString());
        }

        /// <summary>
        /// 发送消息到指定分组
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageToTaskGroup(Guid taskId, string message)
        {
            await Clients.All.SendAsync("UpdateStatus", message);
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            Logger.LogInformation($"UserHubConnected:tenantId:{CurrentTenant.Id},UserId{CurrentUser.Id?.ToString()}{CurrentUser.UserName}Connected SignalR");

            //if (!CurrentUser.Id.HasValue)
            //{
            //    throw new UserFriendlyException("没有用户信息");
            //}
            //var curUserId = CurrentUser.Id.Value;
            //var curUserName = CurrentUser.Name;
            //var curConnectionId = Context.ConnectionId;
            //var curUserDoingJob = new DoingTaskDto()
            //{
            //    ConnectionId = curConnectionId,
            //    UserId = curUserId,
            //    UserName = curUserName,
            //    LastTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            //};

            //await UpdateUserCache(curUserId, curUserDoingJob);

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 重新连接的时候调用
        /// </summary>
        /// <returns></returns>
        //public override Task OnReconnected()
        //{
        //    return base.OnReconnected();
        //}


        private async Task UpdateUserCache(Guid curUserId, DoingTaskDto curUserDoingJob)
        {
            //这里需要记录或是更新这个用户的连接编号，这个用户之前的连接号相关的信息要处理
            var userCacheValue = await _distributedUserCache.GetAsync(curUserId.ToString());

            if (userCacheValue != null)
            {
                if (!userCacheValue.TaskId.Equals(Guid.Empty))
                {
                    //取出之前的数据，因为这个用户有新的连接了，所以要将它从之前的分组中删除
                    var preConnectionId = userCacheValue.ConnectionId;
                    if (preConnectionId != curUserDoingJob.ConnectionId)
                    {
                        await Groups.RemoveFromGroupAsync(preConnectionId, userCacheValue.TaskId.ToString());
                    }
                }
                else
                {
                    //这里说明是用户的新的连接
                }
            }
            else
            {
                await _distributedUserCache.GetOrAddAsync(curUserId.ToString(),
                                               async () =>
                                               {
                                                   return await Task.FromResult(curUserDoingJob);
                                               },
                                           () => new DistributedCacheEntryOptions
                                           {
                                               SlidingExpiration = TimeSpan.FromMinutes(30)
                                           });
            }

            await _distributedUserCache.SetAsync(curUserId.ToString(), curUserDoingJob);

        }

        private async Task RemoveUserCache(Guid curUserId)
        {
            var userCacheValue = await _distributedUserCache.GetAsync(curUserId.ToString());

            if (userCacheValue != null)
            {
                if (!userCacheValue.TaskId.Equals(Guid.Empty))
                {
                    //取出之前的数据，因为这个用户有新的连接了，所以要将它从之前的分组中删除
                    var preConnectionId = userCacheValue.ConnectionId;
                    await Groups.RemoveFromGroupAsync(preConnectionId, userCacheValue.TaskId.ToString());
                }
                await _distributedUserCache.RemoveAsync(curUserId.ToString());
            }
        }

        public async Task<ICollection<Guid>> GetTaskUserIds(Guid taskId, Guid curUserId)
        {
            var userIds = new List<Guid>();
            var taskCacheValue = await _distributedTaskCache.GetAsync(taskId.ToString());
            if (taskCacheValue != null)
            {
                foreach (var doingTask in taskCacheValue.DoingTasks)
                {
                    if (doingTask.Value.UserId != curUserId)
                    {
                        userIds.Add(doingTask.Value.UserId);
                    }
                }
            }

            return userIds;
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Logger.LogInformation($"UserHubDisconnected:tenantId:{CurrentTenant.Id},UserId{CurrentUser.Id?.ToString()}{CurrentUser.UserName} Disconnected signalR，ExceptionMessage：{exception?.Message}");

            //var curUserId = CurrentUser.Id.Value;
            //var curUserName = CurrentUser.Name;
            //var curConnectionId = Context.ConnectionId;
            //await RemoveUserCache(curUserId);

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 心跳检测用于前端判断是否重连
        /// </summary>
        /// <returns></returns>
        public Task Heartbeat()
        {
            // 心跳检测成功，返回空响应
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取任务的操作信息
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<ResultMessage> GetTaskDoing(DoingTaskDto input)
        {
            var result = new ResultMessage();
            Logger.LogInformation("SignalR Hub:GetTaskDoing," + input.TaskId);

            result.Success = true;
            try
            {
                if (!input.UserId.Equals(Guid.Empty))
                {
                    var curUserId = input.UserId;
                    var curUserName = input.UserName;// await _accountManager.GetNameAsync(curUserId);

                    var curUserDoingJob = new DoingTaskDto()
                    {
                        ConnectionId = Context.ConnectionId,
                        UserId = curUserId,
                        UserName = curUserName,
                        TaskId = input.TaskId,
                        LastTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    //更新当前用户的缓存信息
                    {
                        await UpdateUserCache(curUserId, curUserDoingJob);
                    }

                    var taskCacheValue = await _distributedTaskCache.GetAsync(input.TaskId.ToString());
                    if (taskCacheValue == null)
                    {
                        taskCacheValue = new DoingTaskCacheItemDto
                        {
                            TaskId = input.TaskId,
                            DoingTasks = new Dictionary<Guid, DoingTaskDto> { { curUserId, curUserDoingJob } }
                        };

                    }
                    else
                    {
                        //已存在数据，判断当前用户是否有
                        if (taskCacheValue.DoingTasks.ContainsKey(curUserId))
                        {
                            //用户数据已经存在，则更新
                            taskCacheValue.DoingTasks[curUserId] = curUserDoingJob;
                        }
                        else
                        {
                            taskCacheValue.DoingTasks.Add(curUserId, curUserDoingJob);
                        }

                    }
                    await _distributedTaskCache.SetAsync(input.TaskId.ToString(), taskCacheValue);

                    result.Data = taskCacheValue.DoingTasks.Values.ToList();
                    result.Message = "获取成功";

                    await Groups.AddToGroupAsync(Context.ConnectionId, input.TaskId.ToString());
                }
            }
            catch (Exception ex)
            {
                result.Message = "获取任务的操作数据出错：" + ex.Message;
                result.Success = false;
            }

            return result;
        }

        /// <summary>
        /// 开始执行任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ResultMessage> DoingTask(DoingTaskDto input)
        {
            var result = new ResultMessage();

            //using (await _Mutex.LockAsync())
            {

                if (!input.UserId.Equals(Guid.Empty))
                {
                    var curUserId = input.UserId;
                    var curUserName = input.UserName;//await _accountManager.GetNameAsync(curUserId);

                    var curUserDoingJob = new DoingTaskDto()
                    {
                        ConnectionId = Context.ConnectionId,
                        UserId = curUserId,
                        UserName = curUserName,
                        TaskId = input.TaskId,
                        TaskCategoryId = input.TaskCategoryId,
                        TaskItemId = input.TaskItemId,
                        TaskItemDetailId = input.TaskItemDetailId,
                        LastTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    //更新当前用户的缓存信息
                    {
                        await UpdateUserCache(curUserId, curUserDoingJob);
                    }

                    //更新当前任务的用户操作数据
                    {
                        var taskCacheValue = await _distributedTaskCache.GetOrAddAsync(
                                input.TaskId.ToString(),
                                async () =>
                                {
                                    return await Task.FromResult(new DoingTaskCacheItemDto
                                    {
                                        TaskId = input.TaskId,
                                        DoingTasks = new Dictionary<Guid, DoingTaskDto> { { curUserId, curUserDoingJob } }
                                    });
                                },
                                () => new DistributedCacheEntryOptions
                                {
                                    SlidingExpiration = TimeSpan.FromMinutes(30)
                                });

                        if (taskCacheValue.DoingTasks.ContainsKey(curUserId))
                        {
                            taskCacheValue.DoingTasks[curUserId] = curUserDoingJob;
                        }
                        else
                        {
                            taskCacheValue.DoingTasks.Add(curUserId, curUserDoingJob);
                        }

                        await _distributedTaskCache.SetAsync(input.TaskId.ToString(), taskCacheValue);

                        var doingTasks = taskCacheValue.DoingTasks.Values.ToList();

                        result.Data = doingTasks;
                    }


                    var inputData = input.TaskItemDetail;
                    if (inputData != null)
                    {
                        inputData.JobTaskItemId = input.TaskItemId;

                        //await _jobTaskAppService.SaveTaskItemDetailAsync(inputData);
                    }

                    result.Message = "处理更新操作数据成功";
                    result.Success = true;

                    var userIds = await GetTaskUserIds(input.TaskId, input.UserId);

                    await Clients.AllExcept(Context.ConnectionId).SendAsync("UpdateStatus",
                        new
                        {
                            WithUserId = curUserId,
                            WithUserName = curUserName,
                            TaskId = input.TaskId.ToString(),
                            Action = "Doing",
                            Data = curUserDoingJob
                        });

                    Logger.LogInformation("SignalR Hub:UpdateStatus,Doing");

                    //await Groups.AddToGroupAsync(Context.ConnectionId, input.TaskId.ToString());
                }
                else
                {
                    result.Message = "没有获取到当前用户的数据信息";
                    result.Success = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 用户退出当前任务的操作
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ResultMessage> ExitDoingTask(DoingTaskDto input)
        {
            var result = new ResultMessage();

            //using (await _Mutex.LockAsync())
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, input.TaskId.ToString());

                if (!input.UserId.Equals(Guid.Empty))
                {
                    var curUserId = input.UserId;
                    var curUserName = input.UserName;

                    //更新当前用户的缓存信息
                    {
                        await _distributedUserCache.RemoveAsync(curUserId.ToString());
                    }

                    //更新当前任务的用户操作数据
                    {
                        var taskCacheValue = await _distributedTaskCache.GetAsync(input.TaskId.ToString());

                        if (taskCacheValue != null && taskCacheValue.DoingTasks.ContainsKey(curUserId))
                        {
                            taskCacheValue.DoingTasks.Remove(curUserId);

                            await _distributedTaskCache.SetAsync(input.TaskId.ToString(), taskCacheValue);

                            //呼唤一下这个任务的其它用户，更新下当前的操作状态
                            var doingTasks = taskCacheValue.DoingTasks.Values.ToList();

                            result.Data = doingTasks;
                        }
                    }

                    var inputData = input.TaskItemDetail;
                    if (inputData != null)
                    {
                        inputData.JobTaskItemId = input.TaskItemId;

                        //await _jobTaskAppService.SaveTaskItemDetailAsync(inputData);
                    }

                    result.Message = "用户退出操作数据成功";
                    result.Success = true;

                    await Clients.AllExcept(Context.ConnectionId).SendAsync("UpdateStatus",
                        new
                        {
                            WithUserId = curUserId,
                            WithUserName = curUserName,
                            TaskId = input.TaskId.ToString(),
                            Action = "Exit",
                            Data = input
                        });
                    Logger.LogInformation("SignalR Hub:UpdateStatus,Exit");

                }
                else
                {
                    result.Message = "没有获取到当前用户的数据信息";
                    result.Success = false;
                }
            }

            return result;
        }

        /// <summary>
        /// 保存任务操作
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ResultMessage> SaveTaskDoing(DoingTaskDto input)
        {
            Logger.LogInformation("SignalR Hub:SaveTaskDoing,input:{input}", input);

            var result = new ResultMessage();
            //using (await _Mutex.LockAsync())
            {
                if (!input.UserId.Equals(Guid.Empty))
                {
                    var curUserId = input.UserId;
                    var curUserName = input.UserName;

                    var inputData = input.TaskItemDetail;
                    if (inputData != null)
                    {
                        inputData.JobTaskItemId = input.TaskItemId;

                        input.TaskItemDetail = inputData;
                    }

                    var curUserDoingJob = new DoingTaskDto()
                    {
                        ConnectionId = Context.ConnectionId,
                        UserId = curUserId,
                        UserName = curUserName,
                        TaskId = input.TaskId,
                        TaskCategoryId = input.TaskCategoryId,
                        TaskItemId = input.TaskItemId,
                        TaskItemDetailId = input.TaskItemDetailId,
                        TaskItemDetail = input.TaskItemDetail,
                        LastTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    //更新当前用户的缓存信息
                    await UpdateUserCache(curUserId, curUserDoingJob);

                    //更新当前任务的用户操作数据
                    {
                        var taskCacheValue = await _distributedTaskCache.GetOrAddAsync(
                                input.TaskId.ToString(),
                                async () =>
                                {
                                    return await Task.FromResult(new DoingTaskCacheItemDto
                                    {
                                        TaskId = input.TaskId,
                                        DoingTasks = new Dictionary<Guid, DoingTaskDto> { { curUserId, curUserDoingJob } }
                                    });
                                },
                                () => new DistributedCacheEntryOptions
                                {
                                    SlidingExpiration = TimeSpan.FromMinutes(30)
                                });

                        if (taskCacheValue != null)
                        {
                            if (taskCacheValue.DoingTasks.ContainsKey(curUserId))
                            {
                                taskCacheValue.DoingTasks[curUserId] = curUserDoingJob;
                            }
                            else
                            {
                                taskCacheValue.DoingTasks.Add(curUserId, curUserDoingJob);
                            }

                            await _distributedTaskCache.SetAsync(input.TaskId.ToString(), taskCacheValue);
                            var doingTasks = taskCacheValue.DoingTasks.Values.ToList();
                        }


                        result.Data = curUserDoingJob;
                    }


                    result.Message = "保存更新操作数据成功";
                    result.Success = true;
                }
                else
                {
                    result.Message = "没有获取到当前用户的数据信息";
                    result.Success = false;
                }
            }

            return result;
        }

        /// <summary>
        /// 退出整个任务的操作
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ResultMessage> ExitDoing(Guid userId)
        {
            var result = new ResultMessage();

            if (!userId.Equals(Guid.Empty))
            {
                var curUserId = userId;

                Guid? curTaskId = null;

                //更新当前用户的缓存信息
                {
                    var userCacheValue = await _distributedUserCache.GetAsync(curUserId.ToString());

                    if (userCacheValue != null)
                    {
                        curTaskId = userCacheValue.TaskId;
                        await _distributedUserCache.RemoveAsync(curUserId.ToString());
                    }
                }

                //更新当前任务的用户操作数据
                //需要将这个用户从任务分组中删除
                if (curTaskId.HasValue)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, curTaskId.Value.ToString());

                    await Clients.AllExcept(Context.ConnectionId).SendAsync("UpdateStatus",
                        new
                        {
                            WithUserId = curUserId,
                            WithUserName = CurrentUser.Name,//await _accountManager.GetNameAsync(curUserId),
                            TaskId = curTaskId.ToString(),
                            Action = "ExitDoing"
                        });
                    Logger.LogInformation("SignalR Hub:UpdateStatus,ExitDoing");

                    var taskCacheValue = await _distributedTaskCache.GetAsync(curTaskId.Value.ToString());

                    //需要将这个用户的操作相关数据删除
                    if (taskCacheValue != null && taskCacheValue.DoingTasks.ContainsKey(curUserId))
                    {
                        taskCacheValue.DoingTasks.Remove(curUserId);
                        await _distributedTaskCache.SetAsync(curTaskId.Value.ToString(), taskCacheValue);
                    }
                }
                result.Success = true;
                result.Message = "处理退出任务操作成功。";

            }
            else
            {
                result.Success = false;
                result.Message = "处理退出任务操作失败。";
            }

            return result;
        }
    }

    public class ResultMessage
    {
        public bool Success { get; set; } = false;

        public string Message { get; set; } = string.Empty;

        public object? Data { get; set; } = null;
    }
}
