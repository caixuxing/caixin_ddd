using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CaiXin.Domain.MongEntity.Sys
{
    /// <summary>
	/// 系统请求日志
	/// </summary>
	public class SysRequestLog
    {
        public ObjectId Id { get; set; }
        /// <summary>
        /// 业务ID
        /// </summary>
        public string UrlPath { get; set; }

        public string ParameterValues { get; set; }
        /// <summary>
        /// 控制器
        /// </summary>
        public string ControllerName { get; set; }
        /// <summary>
        /// 方法名
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        public string RequestMethod { get; set; }
        /// <summary>
        /// 服务器环境
        /// </summary>
        public string EnvironmentName { get; set; }
        /// <summary>
        /// 完成情况
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 执行耗时
        /// </summary>
        public long ElapsedTime { get; set; }
        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIp { get; set; }

        public string ExceptionMsg { get; set; }

        public string ResponseData { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }
    }
}
