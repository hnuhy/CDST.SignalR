using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDST.SignalR.Jobs
{
    public class DoingTaskCacheItemDto
    {
        public Guid TaskId { get; set; }

        public Dictionary<Guid, DoingTaskDto> DoingTasks { get; set; } = new Dictionary<Guid, DoingTaskDto> { };
    }
}
