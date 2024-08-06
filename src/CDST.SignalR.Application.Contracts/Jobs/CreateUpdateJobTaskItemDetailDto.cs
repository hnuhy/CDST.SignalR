using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace CDST.SignalR.Jobs
{
    [Serializable]
    public class CreateUpdateJobTaskItemDetailDto : ExtensibleEntityDto, IHasConcurrencyStamp
    {
        public virtual Guid? Id { get; set; }

        /// <summary>
        /// 并发标识
        /// </summary>
        public virtual string ConcurrencyStamp { get; set; } = string.Empty;

        /// <summary>
        /// 删除
        /// </summary>
        public virtual bool IsDeleted { get; set; } = false;

        /// <summary>
        /// 是可删除的
        /// </summary>
        public virtual bool IsDeletable { get; set; } = true;

        /// <summary>
        /// 执行任务验证项目编号
        /// </summary>
        public Guid? JobTaskItemId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public virtual int OrderNumber { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        public virtual string? Category { get; set; }

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
        /// 结果
        /// </summary>
        public virtual string? Result { get; set; }

        /// <summary>
        /// 结论
        /// </summary>
        public virtual string? Conclusion { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public virtual object? AttachmentFile { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V1 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V2 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V3 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V4 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V5 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V6 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V7 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V8 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V9 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V10 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V11 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V12 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V13 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V14 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V15 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V16 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V17 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V18 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V19 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V20 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V21 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V22 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V23 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V24 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V25 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V26 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V27 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V28 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V29 { get; set; }

        /// <summary>
        /// 值数据
        /// </summary>
        public virtual string? V30 { get; set; }
    }
}
