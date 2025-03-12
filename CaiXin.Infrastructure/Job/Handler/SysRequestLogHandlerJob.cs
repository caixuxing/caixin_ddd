using CaiXin.Domain.Job;
using CaiXin.Domain.Shared.Const;
using CaiXin.Infrastructure.Job.ArgsDto;
using Hangfire.Server;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using MongoDB.Driver;
using Hangfire.Logging;
using CaiXin.Domain.MongEntity.Sys;

namespace CaiXin.Infrastructure.Job.Handler
{
    
   

    /// <summary>
    /// 系统请求日志
    /// </summary>
    [Dependency(ServiceLifetime.Singleton, ReplaceServices = true)]
    [ExposeServices(typeof(IJob<SysRequestLogArgs>))]
    public class SysRequestLogHandlerJob : ISingletonDependency, IJob<SysRequestLogArgs>
    {
        private readonly IMongoDatabase _mongodb;
        public SysRequestLogHandlerJob(IMongoDatabase mongodb)
        {
            _mongodb = mongodb;
        }
        [Queue(QueueConst.Background)]
        [JobDisplayName("消费系统请求日志入库处理")]
        public async Task<bool> ExecuteAsync(PerformContext context, SysRequestLogArgs args)
        {

           var entity= new SysRequestLog() {
           
               ActionName=args.ActionName,
               ClientIp=args.ClientIp,
               ControllerName=args.ControllerName,
               CreateTime=DateTime.Now,
               ElapsedTime=args.ElapsedTime,
               EnvironmentName=args.EnvironmentName,
               ExceptionMsg=args.ExceptionMsg,
               IsSuccess=args.IsSuccess,
               ParameterValues=args.ParameterValues,
               RequestMethod=args.RequestMethod,
               ResponseData=args.ResponseData,
               UrlPath=args.UrlPath
           };
            await _mongodb.GetCollection<SysRequestLog>(nameof(SysRequestLog))
                          .InsertOneAsync(entity);
            return true;
        }
    }
}
