using CaiXin.Domain.Job;
using CaiXin.Domain.Shared.Const;
using CaiXin.Infrastructure.Job.ArgsDto;
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace CaiXin.Infrastructure.Job.Handler
{
   
    /// <summary>
    /// 三方远程服务队列任务Job
    /// </summary>
    [Dependency(ServiceLifetime.Singleton, ReplaceServices = true)]
    [ExposeServices(typeof(IJob<RemoteServiceArgs>))]
    public class RemoteServiceJob : ISingletonDependency, IJob<RemoteServiceArgs>
    {

        private readonly IHttpClientFactory _httpClientFactory;
        public RemoteServiceJob(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        //[Queue("background-queue")]
        [Queue(QueueConst.Background)]
        [JobDisplayName("远程服务队列任务消费")]
        public async Task<bool> ExecuteAsync(PerformContext context,RemoteServiceArgs args)
        {
            string result = string.Empty;
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(15);
                var content = new StringContent(args.JosnData, System.Text.Encoding.UTF8, "application/json");

                //await Task.Delay(15000);
                var response = await client.PostAsync(args.CallbackUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($@"远程请求地址:{args.CallbackUrl},响应失败,状态码:{response.StatusCode}");
                }
                result = await response.Content.ReadAsStringAsync();
                // 获取任务 ID
                string jobId = context.BackgroundJob.Id;
                Console.WriteLine($"消费处理已完成{jobId}");
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($@"远程请求地址:{args.CallbackUrl} 原始返回结果:{result} 解析异常错误信息:{ex.Message}");
            }
        }
    }


    /// <summary>
    /// Operate Result
    /// </summary>
    public class OperateResult
    {
        /// <summary>
        /// code
        /// </summary>
        public OperateCodeEnum Code { set; get; }
        /// <summary>
        /// msg
        /// </summary>
        public string Msg { set; get; } = "";
    }

    /// <summary>
    /// OperateCodeEnum
    /// </summary>
    public enum OperateCodeEnum
    {
        /// <summary>
        /// Success=0
        /// </summary>
        Success = 0,
        /// <summary>
        /// Fail=-1
        /// </summary>
        Fail = -1,
        /// <summary>
        /// Exception=-9
        /// </summary>
        Exception = -9,
    }
}
