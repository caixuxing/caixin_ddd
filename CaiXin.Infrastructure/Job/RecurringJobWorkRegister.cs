using CaiXin.Domain.Job;
using CaiXin.Infrastructure.Job.ArgsDto;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Infrastructure.Job
{

    /// <summary>
    /// 周期任务工作者
    /// </summary>
    public static class RecurringJobWorkRegister
    {

        /// <summary>
        /// 注册周期任务工作者
        /// </summary>
        /// <param name="context"></param>
        public static void RegisterRecurringJobWorks(IServiceScope scope)
        {
            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/1 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });




            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log2", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/2 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });


            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log3", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/1 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log4", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/1 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log5", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/1 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log6", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/1 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log7", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/1 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log8", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/1 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log9", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/1 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log10", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/1 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log11", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/1 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log12", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/1 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log13", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/1 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log14", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/1 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log15", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/1 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            ///定时清理日志
            RecurringJob.AddOrUpdate("test-log16", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!, new CleaLogsArgs() { }),
                     "*/1 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

        }

    }
}
