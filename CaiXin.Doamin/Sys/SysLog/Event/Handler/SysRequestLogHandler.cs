using CaiXin.Domain.Sys.SysLog.Entity;
using CaiXin.Domain.Sys.SysLog.Event.Agrs;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace CaiXin.Domain.Sys.SysLog.Event.Handler
{

    /// <summary>
    /// 系统请求日志处理
    /// </summary>
    public class SysRequestLogHandler : ILocalEventHandler<SysRequestLogEo>, ITransientDependency
    {
        public readonly ISqlSugarClient _client;

        private readonly ILogger<SysRequestLogHandler> _logger;
        public SysRequestLogHandler(ISqlSugarClient client, ILogger<SysRequestLogHandler> logger)
        {
            _client = client;
            _logger = logger;
        }
        public async Task HandleEventAsync(SysRequestLogEo eo)
        {
            var entity = SysRequestLogDo.Create(
                 urlPath: eo.UrlPath,
                 parameterValues: eo.ParameterValues,
                 controllerName: eo.ControllerName,
                 actionName: eo.ActionName,
                 requestMethod: eo.RequestMethod,
                 environmentName: eo.EnvironmentName,
                 isSuccess: eo.IsSuccess,
                 elapsedTime: eo.ElapsedTime,
                 clientIp: eo.ClientIp,
                 exceptionMsg: eo.ExceptionMsg,
                 responseData: eo.ResponseData
                 );
            await _client.Insertable(entity).ExecuteCommandAsync();
        }
    }
}
