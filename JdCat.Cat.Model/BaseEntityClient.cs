using JdCat.Cat.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JdCat.Cat.Model
{
    public abstract class BaseEntityClient : BaseEntity
    {
        /// <summary>
        /// 本地数据库唯一标识
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }
        /// <summary>
        /// 同步时间
        /// </summary>
        public DateTime SyncTime { get; set; }
        /// <summary>
        /// 实体状态
        /// </summary>
        public EntityStatus Status { get; set; }
    }
}
