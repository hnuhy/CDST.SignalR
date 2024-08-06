using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace CDST.SignalR.Jobs
{
    /// <summary>
    /// 执行任务项
    /// </summary>
    [Serializable]
    public class CreateUpdateJobTaskItemDto : ExtensibleEntityDto, IHasConcurrencyStamp
    {
        public virtual Guid? Id { get; set; }

        /// <summary>
        /// 并发标识
        /// </summary>
        public virtual string ConcurrencyStamp { get; set; } = string.Empty;


        /// <summary>
        /// 执行任务分组编号
        /// </summary>
        public Guid JobTaskCategoryId { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        public virtual string? Category { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string? Name { get; set; }

        /// <summary>
        /// 点位
        /// </summary>
        public virtual string? Position { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public virtual string? Description { get; set; }

        /// <summary>
        /// 结论
        /// </summary>
        public virtual string? Conclusion { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public virtual object? AttachmentFile { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public virtual int OrderNumber { get; set; }

        /// <summary>
        /// 是否标记
        /// </summary>
        public virtual bool IsMarked { get; set; } = false;

        /// <summary>
        /// 标准操作程序
        /// </summary>
        public virtual string? SOP { get; set; } = string.Empty;

        /// <summary>
        /// 操作指南
        /// </summary>
        public virtual string? Guide { get; set; } = string.Empty;

        /// <summary>
        /// 数据录入表单内容
        /// </summary>
        public virtual string? FormContent { get; set; }


        /// <summary>
        /// 数据录入表单
        /// </summary>
        //public FormDto? Form { get; set; }

        /// <summary>
        /// 详细数据
        /// </summary>
        public ICollection<CreateUpdateJobTaskItemDetailDto> TaskItemDetails { get; set; } = new List<CreateUpdateJobTaskItemDetailDto>();

    }
}
