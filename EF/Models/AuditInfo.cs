using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuditLogDemo.Models
{
    public class AuditInfo
    {

        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 调用参数
        /// </summary>
        public string Parameters { get; set; }
        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string BrowserInfo { get; set; }
        /// <summary>
        /// 客户端信息
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public string ClientIpAddress { get; set; }
        /// <summary>
        /// 执行耗时
        /// </summary>
        public int ExecutionDuration { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime ExecutionTime { get; set; }
        /// <summary>
        /// 返回内容
        /// </summary>
        public string ReturnValue { get; set; }
        /// <summary>
        /// 异常对象
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 调用者信息
        /// </summary>
        public string UserInfo { get; set; }
        /// <summary>
        /// 自定义数据
        /// </summary>
        public string CustomData { get; set; }
    }
}
