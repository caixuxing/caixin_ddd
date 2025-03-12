using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Domain.Job
{
    public interface IJob<in TArgs>
    {
        public Task<bool> ExecuteAsync(PerformContext context,TArgs args);
    }
}
