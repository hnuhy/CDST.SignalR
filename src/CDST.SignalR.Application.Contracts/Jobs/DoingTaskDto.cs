using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDST.SignalR.Jobs
{
    public class DoingTaskDto
    {
        public string? ConnectionId { get; set; }

        public Guid UserId { get; set; }

        public virtual string? UserName { get; set; }

        public virtual string? LastTime { get; set; }

        public Guid TaskId { get; set; }

        public Guid? TaskCategoryId { get; set; }

        public Guid? TaskItemId { get; set; }

        public Guid? TaskItemDetailId { get; set; }

        public virtual CreateUpdateJobTaskItemDetailDto? TaskItemDetail { get; set; }
    }
}
