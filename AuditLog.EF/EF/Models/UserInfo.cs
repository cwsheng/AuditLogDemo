using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AuditLog.EF.EF.Models
{
    [Table("UserInfos")]
    public class UserInfo
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 客户端信息
        /// </summary>
        public string UserName { get; set; }
        public string UserPwd { get; set; }
    }
}
