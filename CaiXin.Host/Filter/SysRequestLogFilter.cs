using CaiXin.Domain.Job;
using CaiXin.Domain.Shared.Response;
using CaiXin.Domain.Sys.SysLog.Event.Agrs;
using CaiXin.Infrastructure.Job.ArgsDto;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using Volo.Abp.EventBus.Local;

namespace CaiXin.Host.Filter
{
    /// <summary>
    /// 系统请求日志
    /// </summary>
    public class SysRequestLogFilter : IAsyncActionFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 获取控制器/操作描述器
            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            ArgumentNullException.ThrowIfNull(controllerActionDescriptor);
            // 获取 HttpContext 和 HttpRequest 对象
            var httpContext = context.HttpContext;
            var httpRequest = httpContext.Request;
            var feature = httpContext.Features.Get<IHttpConnectionFeature>();
            // 获取客户端 IPv4 地址
            var remoteIPv4 = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault() ?? feature?.LocalIpAddress?.MapToIPv4().ToString();
            var requestMethord = httpRequest.Method;
            // 服务器环境
            var environmentName = httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().EnvironmentName;
            // 获取方法参数
            var parameterValues = context.ActionArguments;

            var sw = Stopwatch.StartNew();
            var resultContext = await next();
            sw.Stop();
            var data = new SysRequestLogArgs
            {
                UrlPath = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.Path}{httpRequest.QueryString}",
                ParameterValues = Newtonsoft.Json.JsonConvert.SerializeObject(parameterValues),
                ActionName = controllerActionDescriptor.ActionName,
                ControllerName = controllerActionDescriptor.ControllerName,
                ClientIp = remoteIPv4 ?? string.Empty,
                RequestMethod = requestMethord,
                ElapsedTime = sw.ElapsedMilliseconds,
                EnvironmentName = environmentName,
                IsSuccess = resultContext?.Exception == null,
                ExceptionMsg = resultContext?.Exception?.Message ?? string.Empty,
                ResponseData = Newtonsoft.Json.JsonConvert.SerializeObject((resultContext?.Result as Microsoft.AspNetCore.Mvc.ObjectResult)?.Value)

            };
            if (httpContext.GetEndpoint()?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null)
            {
                return;
            }
            BackgroundJob.Enqueue(() => httpContext.RequestServices.GetRequiredService<IJob<SysRequestLogArgs>>().ExecuteAsync(default!, data));
        }
    }
}
