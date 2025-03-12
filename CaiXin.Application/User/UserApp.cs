using CaiXin.Application.Contracts.UserApp;
using CaiXin.Application.Contracts.UserApp.Commands;
using CaiXin.Application.Contracts.UserApp.Dto;
using CaiXin.Application.Contracts.UserApp.Query;
using CaiXin.Application.Contracts.Validate;
using CaiXin.Domain.Job;
using CaiXin.Domain.Shared.Const;
using CaiXin.Domain.SysUser.Entity;
using CaiXin.Infrastructure.Extensions;
using CaiXin.Infrastructure.Job.ArgsDto;
using CaiXin.Infrastructure.MessageBroker;
using CaiXin.Infrastructure.Uilts;
using FluentValidation;
using Hangfire;
using Microsoft.Extensions.Caching.Distributed;
using SqlSugar;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Volo.Abp.Application.Services;
using Volo.Abp.Validation;

namespace CaiXin.Application.User
{
    /// <summary>
    /// UserApp 
    /// </summary>
    [DisableValidation]
    public  class UserApp : ApplicationService, IUserApp
    {
        private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();
        /// <summary>
        /// 用户仓储
        /// </summary>
        private ISimpleClient<UserDo> userRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<UserDo>>();


        private IMessagePublisher Publisher => LazyServiceProvider.LazyGetRequiredService<IMessagePublisher>();

        private IDistributedCache Cache => LazyServiceProvider.LazyGetRequiredService<IDistributedCache>();


        private ConnectionMultiplexer ConnectionMultiplexer => LazyServiceProvider.LazyGetRequiredService<ConnectionMultiplexer>();

        public async Task<bool> CheckUserNameExistsAsync(string userName)=>  await userRepo.IsAnyAsync(x => x.Name.Equals(userName));





        /// <summary>
        /// 异步创建用户的方法
        /// </summary>
        /// <param name="cmd">包含创建用户所需信息的命令对象，该参数不可为 null</param>
        /// <returns>返回一个异步任务，任务完成后返回新创建用户的 ID</returns>
        public async Task<long> CreateUserAsync([NotNull] CreateUserCmd cmd)
        {
            // 校验创建用户模型指令
            // 从 LazyServiceProvider 中延迟获取 CreateUserCmd 类型的验证器实例，
            // 并异步验证传入的 cmd 对象。如果验证失败，会抛出异常。
            await LazyServiceProvider.LazyGetRequiredService<IValidator<CreateUserCmd>>().ValidateAndThrowAsync(cmd);

            // 创建用户
            // 调用 UserDo 类的 CreateUser 方法，根据 cmd 中的用户名、密码和指定的邮箱创建用户实体对象。
            // 如果创建失败，即返回 null，则抛出 InvalidOperationException 异常。
            var entity = UserDo.CreateUser(cmd.UserName, cmd.PassWord, "11360847@qq.com")
                ?? throw new InvalidOperationException("用户创建失败！");
            // 为创建的用户实体设置账户信息
            entity.SetAccount("caixin")
                 .SetPersonInfo( JsonSerializer.Serialize(new { id = 1, name = "ccx", ct = new List<string>() { "1", "3" } }));

                           // 序列化对象
                var userJson = JsonSerializer.Serialize(entity);

            // 执行事务
            // 调用 TransactionUtils 类的 ExecuteInTransactionAsync 方法，
            // 在事务中执行异步操作。db 是数据库上下文对象。
            var result = await TransactionUtils.ExecuteInTransactionAsync<long>(db, async () =>
            {
                // 用户实体保存到数据库
                // 调用 userRepo 仓储对象的 InsertReturnBigIdentityAsync 方法，
                // 将用户实体插入数据库，并返回新插入记录的自增 ID。
                var resultId = await userRepo.InsertReturnBigIdentityAsync(entity);
                // 如果返回的 ID 小于等于 0，说明插入操作失败，抛出异常。
                if (resultId <= 0) throw new InvalidOperationException("用户信息保存失败！");

                // 创建后台任务
                // 使用 BackgroundJob 类的 Enqueue 方法将一个后台作业入队。
                // 该作业会调用 LazyServiceProvider 中延迟获取的 IJob<SysRequestLogArgs> 服务的 ExecuteAsync 方法，
                // 并传入一个包含操作名称的 SysRequestLogArgs 对象。
                var JobId = BackgroundJob.Enqueue(() =>
                    LazyServiceProvider.LazyGetRequiredService<IJob<SysRequestLogArgs>>().ExecuteAsync(default!,
                        new SysRequestLogArgs()
                        {
                            ActionName = "ccx"
                        }));
                // 如果返回的作业 ID 为空或仅包含空白字符，说明后台作业入队失败，抛出异常，事务会回滚。
                if (string.IsNullOrWhiteSpace(JobId)) throw new InvalidOperationException("后台作业入队失败,数据已回滚!");

                // 返回新创建用户的 ID
                return resultId;
            });

            // 返回用户 ID
            return result;
        }


        /// <summary>
        /// 异步根据用户 ID 查找用户信息并将其转换为数据传输对象（DTO）的方法
        /// </summary>
        /// <param name="Id">要查找的用户的唯一标识 ID</param>
        /// <returns>返回一个异步任务，该任务完成后将返回一个表示用户信息的数据传输对象（UserDto）</returns>
        public async Task<UserDto> FindById(long Id)
            // 调用 userRepo 仓储对象的 GetByIdAsync 方法，以异步方式根据传入的用户 ID 从数据源中获取用户实体对象
            // 由于 GetByIdAsync 是异步方法，使用 await 关键字等待该方法执行完成并获取返回的用户实体对象
            // 接着调用该用户实体对象的 MapUserDto 方法，将实体对象映射转换为 UserDto 类型的对象
            => (await userRepo.GetByIdAsync(Id)).MapUserDto();



        /// <summary>
        /// 异步查询用户分页列表的方法
        /// </summary>
        /// <param name="qry">包含用户分页查询条件的查询对象</param>
        /// <returns>返回一个包含用户数据传输对象（UserDto）集合的异步任务</returns>
        public Task<IEnumerable<UserDto>> QueryPageList(UserPageQry qry) =>
            // 从 userRepo 仓储获取可查询对象，用于后续的查询操作
            userRepo.AsQueryable()
            // 使用 WhereIF 方法进行条件筛选。如果 qry.UserName 不为空或仅包含空白字符，
            // 则添加一个筛选条件，筛选出用户名称等于 qry.UserName 的记录
            .WhereIF(!string.IsNullOrWhiteSpace(qry.UserName), x => x.Name.Equals(qry.UserName))
            // 调用 MapUserDtoListAsync 方法将查询结果异步映射为 UserDto 集合
            .MapUserDtoListAsync();


        /// <summary>
        /// 异步更新用户信息的方法
        /// </summary>
        /// <param name="cmd">包含用户更新信息的命令对象</param>
        /// <returns>表示更新操作是否成功的布尔值的异步任务</returns>
        public async Task<bool> UpdateUserAsync(UpdateUserCmd cmd)
        {



            // 校验用户更新模型指令
            // 从 LazyServiceProvider 中延迟获取 UpdateUserCmd 类型的验证器实例，
            // 并异步验证传入的 cmd 对象。如果验证失败，会抛出异常。
            await LazyServiceProvider.LazyGetRequiredService<IValidator<UpdateUserCmd>>().ValidateAndThrowAsync(cmd);

            // 按 ID 查询用户信息
            // 通过 userRepo 仓储对象的 GetByIdAsync 方法，根据 cmd 中的 ID 异步查询用户信息。
            // 由于 cmd.Id 是字符串类型，需要先将其转换为 long 类型。
            // 如果查询结果为 null，说明用户信息不存在，抛出 InvalidOperationException 异常。
            var entity = await userRepo.GetByIdAsync(long.Parse(cmd.Id))
                ?? throw new InvalidOperationException($"用户信息不存在!");

            // 更新用户实体信息
            // 调用 entity 对象的 SetName 方法，将用户名称更新为 cmd 中的 UserName。
            // 然后链式调用 SetPassword 方法，将用户密码更新为 cmd 中的 PassWord。
            entity.SetName(cmd.UserName)
                  .SetPassword(cmd.PassWord);






            //解析Json 转换到实体类中

          var obj=  JsonSerializer.Deserialize<PersonObj>(entity.PersonInfo);




            //执行事务
            var result = await TransactionUtils.ExecuteInTransactionAsync<bool>(db, async () =>
            {
                // 更新影响行
                // 将 entity 对象转换为可更新的对象，
                // 并指定要更新的列，包括名称、密码、版本、最后修改人 ID、最后修改人名称和最后修改时间。
                // 然后异步执行更新操作，并使用乐观锁机制。
                var affectedRows = await userRepo.AsUpdateable(entity)
                .UpdateColumns(it => new { it.Name, it.Password, it.Version, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime })
                .ExecuteCommandWithOptLockAsync();

                // 如果更新影响的行数不是 1，说明更新操作失败，抛出 InvalidOperationException 异常。
                if (affectedRows is not 1) throw new InvalidOperationException($"用户信息更新失败!");

                // 发布消息
                // 异步发布消息，将 entity 对象的字符串表示作为消息内容，
                // 发布到 "11360847_Test" 主题，使用 "caixin.TestKey" 作为键。
                await Publisher.PublishAsync<string>(entity.ToString(), RabbitMqConst.CaiXin_Exchange_User_Event,RabbitMqConst.CaiXin_Routing_User_Test);



                // 序列化对象
                var userJson = JsonSerializer.Serialize(entity);
                var userBytes = Encoding.UTF8.GetBytes(userJson);
                // 缓存选项：设置绝对过期时间（10分钟）
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10));
                await Cache.SetAsync($"{nameof(UserDo)}:{entity.Id}", userBytes, options);



                //从redis 存储读取信息
                var queryUserBytes = await Cache.GetAsync($"{nameof(UserDo)}:{entity.Id}");
                if (queryUserBytes is null) throw new InvalidOperationException($"用户不存在或已过期!");
                var userJsonx = Encoding.UTF8.GetString(queryUserBytes);
                var user = JsonSerializer.Deserialize<UserDo>(userJsonx);


                //将实体对象存储到hash 对象中
               await Cache.SetHashAsync(ConnectionMultiplexer, $"{nameof(UserDo)}:entity:{entity.Id}", entity);

                return true;
            });

            return result;
        }
    }

    public class PersonObj
    {
       
        public int id { get; set; }

        public string name { get; set; }

        public List<string> ct { get; set; }


    }

    /// <summary>
    /// Map扩展
    /// </summary>
    internal static partial class MapExt
    {

        /// <summary>
        /// MapUserDtoList
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        internal static  async Task<IEnumerable<UserDto>> MapUserDtoListAsync(this ISugarQueryable<UserDo> entities)=>
            await entities.Select(x => new UserDto() { Id = x.Id, Name = x.Name }).ToListAsync();

        /// <summary>
        /// MapUserDto
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal static UserDto MapUserDto(this UserDo? entity)=>entity is null? new UserDto() :new UserDto() { Id = entity.Id, Name = entity.Name };
        
        
    }
}