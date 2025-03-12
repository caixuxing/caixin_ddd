using CaiXin.Application.Contracts.UserApp.Commands;
using CaiXin.Application.User;
using CaiXin.Domain.Job;
using CaiXin.Domain.Shared.Response;
using CaiXin.Host.Models;
using CaiXin.Infrastructure.Job.ArgsDto;
using FluentValidation;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using CaiXin.Application.Contracts.Validate;

namespace CaiXin.Host.Controllers
{
    /// <summary>
    /// 队列管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
	public class QueueController : AbpController
    {


        private IJob<RemoteServiceArgs> Job => LazyServiceProvider.LazyGetRequiredService<IJob<RemoteServiceArgs>>();
      
        /// <summary>
        /// 创建队列
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost, Route("Queue/Create")]

        public async Task<IActionResult> Create([FromBody] CreateQueueCmd cmd)
        {
            await LazyServiceProvider.LazyGetRequiredService<IValidator<CreateQueueCmd>>().ValidateAndThrowAsync(cmd);
            var message = BackgroundJob.Enqueue(() => Job.ExecuteAsync(default!,new RemoteServiceArgs()
            {
                JosnData = cmd.JosnData,
                CallbackUrl = cmd.CallbackUrl,
            }));
            if (!string.IsNullOrWhiteSpace(message))
            {
                return Ok(new ApiResult<string>("创建成功", message));
            }
            return Ok(new ApiResult<string>("创建成功", message));
        }
    }
}
