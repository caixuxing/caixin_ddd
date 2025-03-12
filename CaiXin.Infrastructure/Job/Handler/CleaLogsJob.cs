using CaiXin.Domain.Job;
using CaiXin.Domain.Shared.Const;
using CaiXin.Infrastructure.Job.ArgsDto;
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace CaiXin.Infrastructure.Job.Handler
{



    /// <summary>
    /// 清理追房任务运行日志
    /// </summary>
    [Dependency(ServiceLifetime.Singleton, ReplaceServices = true)]
    [ExposeServices(typeof(IJob<CleaLogsArgs>))]
    public class CleaLogsJob : ISingletonDependency, IJob<CleaLogsArgs>
    {
        private readonly IMongoDatabase mongodb;
        public CleaLogsJob(IMongoDatabase mongodb)
        {
            this.mongodb = mongodb;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Queue(QueueConst.Background)]
        [JobDisplayName("清理-OTA追房任务运行日志")]
     
        public async Task<bool> ExecuteAsync(PerformContext context, CleaLogsArgs args)
        {
           await Task.Delay(10000);
            return true;
        }
    }
}
