using CaiXin.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Domain.Shared.Response
{
    /// <summary>
    /// 数据返回模型基类
    /// </summary>
    public class ApiResult<T>
    {
        public ApiResult(MessageType messageType, ResultCode resultCode, string? message, T? data)
        {
            Code = resultCode;
            this.MessageType = messageType.ToString().ToLower();
            Message = message;
            Data = data;
        }
        public ApiResult(string? message, T? data, MessageType messageType = Enums.MessageType.Success, ResultCode resultCode = ResultCode.SUCCESS)
        {
            Code = resultCode;
            this.MessageType = messageType.ToString().ToLower();
            Message = message;
            Data = data;
        }

        public ApiResult()
        { }

        /// <summary>
        /// 返回状态码
        /// </summary>
        public virtual ResultCode Code { get; set; } = ResultCode.SUCCESS;
        /// <summary>
        /// 信息类型
        /// </summary>
        public virtual string MessageType { get; set; } = "none";

        /// <summary>
        /// 获取 消息内容
        /// </summary>
        public virtual string? Message { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public virtual T? Data { get; set; }
    }
}
