using CaiXin.Application.Contracts.UserApp;
using CaiXin.Application.Contracts.UserApp.Commands;
using CaiXin.Domain.Shared.Response;
using CaiXin.Domain.SysUser.Entity;
using CaiXin.Infrastructure.Extensions;
using CaiXin.Infrastructure.MessageBroker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using Volo.Abp.AspNetCore.Mvc;

namespace CaiXin.Host.Controllers
{
    /// <summary>
    /// 用户
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : AbpController
    {
        private IUserApp UserApp => LazyServiceProvider.LazyGetRequiredService<IUserApp>();


        private IDistributedCache Cache => LazyServiceProvider.LazyGetRequiredService<IDistributedCache>();

        private ConnectionMultiplexer ConnectionMultiplexer => LazyServiceProvider.LazyGetRequiredService<ConnectionMultiplexer>();

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>

        [HttpPost, Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserCmd cmd)
        {


           // var mode= await Cache.GetHash<UserDo>(ConnectionMultiplexer, $"UserDo:entity:1");

            var datattx = await Cache.GetHashDataByPatternAsync<UserDo>(ConnectionMultiplexer, "UserDo:entity*");




            return  Ok(new ApiResult<long>("创建成功", await UserApp.CreateUserAsync(cmd)));
        }

        /// <summary>
        /// 更新账户
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>

        [HttpPost, Route("UpAccount")]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateUserCmd cmd)
        {
            return  Ok(new ApiResult<bool>("更新成功", await UserApp.UpdateUserAsync(cmd)));
        }
        

    }
}
