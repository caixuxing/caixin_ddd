using CaiXin.Domain.Job;
using CaiXin.Infrastructure.Job.ArgsDto;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace CaiXin.Infrastructure.Job
{
    public class HangfireJobWorkRegister : BackgroundService
    {



        private readonly IServiceScope scope;

        public HangfireJobWorkRegister(IServiceScope scope)
        {
            this.scope = scope;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            ///定时清理日志
            RecurringJob.AddOrUpdate("", () => scope.ServiceProvider.GetRequiredService<IJob<CleaLogsArgs>>().ExecuteAsync(default!,new CleaLogsArgs() { }),
                     "*/30 * * * * ?", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            throw new NotImplementedException();

        }
    }
}
