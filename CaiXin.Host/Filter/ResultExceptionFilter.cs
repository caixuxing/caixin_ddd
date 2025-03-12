using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.DirectoryServices.Protocols;
using System.Text;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.Authorization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Http;
using Volo.Abp.Json;
using Volo.Abp.Validation;
using Volo.Abp;
using Microsoft.AspNetCore.Mvc.Abstractions;
using CaiXin.Domain.Shared.Enums;
using CaiXin.Domain.Shared.Response;

namespace CaiXin.Host.Filter
{
    /// <summary>
    /// 全局异常统一格式返回
    /// </summary>
    public class ResultExceptionFilter : IAsyncExceptionFilter, ITransientDependency
    {
        private ILogger<ResultExceptionFilter> Logger;
        public ResultExceptionFilter(
            ILogger<ResultExceptionFilter> logger)
        {
            Logger = NullLogger<ResultExceptionFilter>.Instance;
            Logger = logger;
        }
        public async Task OnExceptionAsync(ExceptionContext context)
        {
           
            await HandleAndWrapException(context);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual async Task HandleAndWrapException(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = 200;
            var logLevel = context.Exception.GetLogLevel();
            Domain.Shared.Enums.ResultCode code = Domain.Shared.Enums.ResultCode.CUSTOM_ERROR;
            string errorMsg = string.Empty;
            object? data = null;
            MessageType type = MessageType.None;
            if (context.Exception is InvalidOperationException customException)
            {
                errorMsg = customException.Message;
                code = Domain.Shared.Enums.ResultCode.BAD_REQUEST;
                data = customException.Data;
                type = MessageType.None;
            }
            else
            {
                errorMsg = "服务器好像出了点问题，请联系系统管理员...";
                logLevel = LogLevel.Error;
                type = MessageType.Error;
            }
            ApiResult<object> apiResult = new ApiResult<object>(type, code, errorMsg, data);
            string responseResult = JsonConvert.SerializeObject(apiResult, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
            var remoteServiceErrorInfoBuilder = new StringBuilder();
            remoteServiceErrorInfoBuilder.AppendLine($"---------- {nameof(RemoteServiceErrorInfo)} ----------");
            remoteServiceErrorInfoBuilder.AppendLine(responseResult);
            Logger.LogWithLevel(logLevel, remoteServiceErrorInfoBuilder.ToString());
            Logger.LogWithLevel(logLevel, "---------- 错误明细 ----------");
            Logger.LogException(context.Exception, logLevel);
            context.Exception = null;
            context.ExceptionHandled = true;
            context.HttpContext.Response.ContentType = "text/json;charset=utf-8";
            await context.HttpContext.Response.WriteAsync(responseResult, System.Text.Encoding.UTF8);
        }

    }
}
