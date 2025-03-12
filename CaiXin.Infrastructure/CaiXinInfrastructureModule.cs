using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar.DistributedSystem.Snowflake;
using SqlSugar;
using System.Reflection;
using CaiXin.Domain.AggRoot;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SqlSugar.IOC;
using CaiXin.Domain.Shared.Const;
using CaiXin.Domain;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo;
using Hangfire;
using MongoDB.Driver;
using CaiXin.Domain.Shared.Config;
using Hangfire.Console;
using Hangfire.HttpJob;
using Hangfire.Dashboard;
using Hangfire.Dashboard.BasicAuthorization;
using RabbitMQ.Client;
using CaiXin.Infrastructure.MessageBroker;
using DeviceDetectorNET.Parser.Device;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using static IdentityModel.ClaimComparer;
using NLog.Config;
using static Org.BouncyCastle.Math.EC.ECCurve;
using CaiXin.Infrastructure.Job;
using Hangfire.Redis.StackExchange;
using Microsoft.Extensions.Hosting;

namespace CaiXin.Infrastructure
{
   


    [DependsOn(
        typeof(CaiXinDomainModule)
        )]
    public class CaiXinInfrastructureModule : AbpModule
    {
        public override   Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {




       

        var multiplexer =  ConnectionMultiplexer.ConnectAsync(new ConfigurationOptions
        {

            // 添加 Redis 服务器地址（支持多个节点）
            EndPoints = { "192.168.60.130:6379" },
            // 连接密码
            // Password = "your_redis_password",

            // 连接超时（毫秒）
            ConnectTimeout = 5000,

            // 同步操作超时（毫秒）
            SyncTimeout = 5000,

            // 是否允许执行管理员命令（慎用）
            //AllowAdmin = false,

            // 启用 SSL 加密
            //Ssl = true,

            // 自动重连策略
            ReconnectRetryPolicy = new ExponentialRetry(5000), // 重试间隔按指数增长
            AbortOnConnectFail = false // 连接失败时不终止

        });


            // 修改注册代码，确保 Dispose 被调用
            context.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {

                // 注册应用停止时的清理逻辑
                var lifetime = sp.GetRequiredService<IHostApplicationLifetime>();
                lifetime.ApplicationStopping.Register(async () =>
                {
                   (await multiplexer).Close();
                   (await multiplexer).Dispose();
                });
                return multiplexer.Result;
            });



            // 注册RabbitMQ连接工厂
            context.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>(sp =>
            {
                var factory = new ConnectionFactory()
                {
                    HostName = "192.168.65.128",
                    Port = 5672,
                    UserName = "admin",
                    Password = "admin",
                    VirtualHost = "/",
                };
                return new RabbitMQConnection(factory);
            });

            context.Services.AddTransient<IMessagePublisher, RabbitMqPublisher>();
            context.Services.AddHostedService<RabbitMQConsumerService>();
            context.Services.AddHostedService<RabbitMQConsumerServiceTest>();



            var redisConfig = new ConfigurationOptions
            {
                EndPoints = { "192.168.65.128:6379" },
                Password = "123456",
                ConnectTimeout = 5000,
                SyncTimeout = 10000,
                DefaultDatabase = 2,
            };

            context.Services.AddStackExchangeRedisCache(option =>
            {
                option.ConnectionMultiplexerFactory = async () => await ConnectionMultiplexer.ConnectAsync(redisConfig);
                option.InstanceName = "SampleInstance_"; // 缓存键前缀

            });

            var connectionMultiplexer = ConnectionMultiplexer.ConnectAsync(redisConfig);
            context.Services.AddSingleton( async() => await connectionMultiplexer );


            var configuration = context.Services.GetConfiguration();

            var mongodbConfig = configuration.GetSection("MongoDb").Get<MongoDbConfig>();
            //===============================Hangfire=================================================

            if (mongodbConfig is not null)
            {
                //var mongoUrlBuilder = new MongoUrlBuilder(mongodbConfig?.ConnectionString);
                //var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
                //GlobalConfiguration.Configuration.UseMongoStorage(mongoClient, mongodbConfig?.DatabaseName, new MongoStorageOptions
                //{
                //    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection,
                //    MigrationOptions = new MongoMigrationOptions
                //    {
                //        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                //        BackupStrategy = new CollectionMongoBackupStrategy()
                //    },
                //    Prefix = "hangfire",
                //    QueuePollInterval = TimeSpan.FromSeconds(15), //- 作业队列轮询间隔。默认值为15秒。
                //    JobExpirationCheckInterval = TimeSpan.FromMinutes(1), //- 作业过期检查间隔。默认值为1分钟。
                //    CountersAggregateInterval = TimeSpan.FromMinutes(5), //- 计数器聚合间隔。默认值为5分钟。
                //    CheckConnection = false,


                //    // 设置连接超时时间为 5 秒
                //    ConnectionCheckTimeout = TimeSpan.FromSeconds(5),
                //    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),

                //});

                GlobalConfiguration.Configuration.UseRedisStorage(connectionMultiplexer.Result, new RedisStorageOptions()
                {
                    Db = 9,
                    Prefix = "hangfire",

                });


            }

            var hnagfireConfig = configuration.GetSection("HangfireConfig").Get<HangfireConfig>();
            if (hnagfireConfig is not null && (hnagfireConfig?.ServerEnable ?? false))
            {
                context.Services.AddHangfire(configuration => configuration
                     .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                     .UseSimpleAssemblyNameTypeSerializer()
                     .UseRecommendedSerializerSettings()
                     .UseConsole(new ConsoleOptions { BackgroundColor = "#000000" })
                     .UseHangfireHttpJob(new HangfireHttpJobOptions()
                     {
                         DashboardTitle = "Hangfire 管理",
                         MailOption = new MailOption
                         {
                             //定时任务执行异常邮件账号配置
                             //Server = emailAlarmConfig.Host,
                             //Port = emailAlarmConfig.Port,
                             //User = emailAlarmConfig.From,
                             //Password = emailAlarmConfig.Password,
                             UseSsl = true
                         }
                     })
                     .UseDashboardMetrics([
                DashboardMetrics.ServerCount,
                 DashboardMetrics.RecurringJobCount,
                 DashboardMetrics.RetriesCount,
                 DashboardMetrics.EnqueuedAndQueueCount,
                 DashboardMetrics.EnqueuedCountOrNull,
                 DashboardMetrics.FailedCountOrNull,
                 DashboardMetrics.EnqueuedAndQueueCount,
                 DashboardMetrics.ScheduledCount,
                 DashboardMetrics.ProcessingCount,
                 DashboardMetrics.SucceededCount,
                 DashboardMetrics.FailedCount,
                 DashboardMetrics.DeletedCount,
                 DashboardMetrics.AwaitingCount ])
                 );
                context.Services.AddHangfireServer(options =>
                {
                    options.Queues = new[] { QueueConst.Default, };
                    options.ServerName = $"CaiXinService";
                    options.SchedulePollingInterval = TimeSpan.FromMicroseconds(1000);
                    options.HeartbeatInterval = TimeSpan.FromMicroseconds(1000);
                    options.WorkerCount = 1; //Environment.ProcessorCount;//并发任务数
                });
                context.Services.AddHangfireServer(options =>
                {
                    options.Queues = new[] { QueueConst.Background, QueueConst.Ota }; //队列名称，只能为小写
                    options.ServerName = $"BackgroundService";
                    options.SchedulePollingInterval = TimeSpan.FromMicroseconds(1000);
                    options.HeartbeatInterval = TimeSpan.FromMicroseconds(1000);
                    options.WorkerCount = Environment.ProcessorCount * 5; //并发任务数
                });

            }
            GlobalConfiguration.Configuration.UseNLogLogProvider();




            // 注册雪花算法ID生成器为单例
            context.Services.AddSingleton(new IdWorker(1, 1));
            var connectionConfigs = configuration.GetSection("ConnectionConfigs").Get<IocConfig>();


           var _accesssor = context.Services.GetRequiredServiceLazy<IHttpContextAccessor>();



            context.Services.AddSingleton<ISqlSugarClient>(s =>
            {
                SqlSugarScope sqlSugar = new SqlSugarScope(new SqlSugar.ConnectionConfig()
                {
                    DbType = DbType.SqlServer,
                    ConnectionString = connectionConfigs.ConnectionString,
                    IsAutoCloseConnection = connectionConfigs.IsAutoCloseConnection,

                    MoreSettings = new ConnMoreSettings()
                    {
                        IsAutoRemoveDataCache = true, // 启用自动删除缓存，所有增删改会自动调用.RemoveDataCache()
                        IsAutoDeleteQueryFilter = true, // 启用删除查询过滤器
                        IsAutoUpdateQueryFilter = true, // 启用更新查询过滤器
                        SqlServerCodeFirstNvarchar = true // 采用Nvarchar
                    },
                    ConfigureExternalServices = new ConfigureExternalServices
                    {
                        EntityNameService = (type, entity) => // 处理表
                        {
                            entity.IsDisabledDelete = true; // 禁止删除非 sqlsugar 创建的列
                                                            // 只处理贴了特性[SugarTable]表
                            if (!type.GetCustomAttributes<SugarTable>().Any())
                                return;
                            if (!entity.DbTableName.Contains('_'))
                                entity.DbTableName = UtilMethods.ToUnderLine(entity.DbTableName); // 驼峰转下划线
                        },
                        EntityService = (type, column) => // 处理列
                        {
                            // 只处理贴了特性[SugarColumn]列
                            if (!type.GetCustomAttributes<SugarColumn>().Any())
                                return;
                            if (new NullabilityInfoContext().Create(type).WriteState is NullabilityState.Nullable)
                                column.IsNullable = true;
                            if (!column.IsIgnore && !column.DbColumnName.Contains('_'))
                                column.DbColumnName = UtilMethods.ToUnderLine(column.DbColumnName); // 驼峰转下划线
                        }
                    }
                },
               db =>
               {
                   db.Aop.OnLogExecuting = (sql, pars) =>
                   {
                       var Strsql = new KeyValuePair<string, SugarParameter[]>(sql, pars);
                   };
                   db.Aop.OnLogExecuting = (sql, pars) =>
                   {
                       var log = $"【{DateTime.Now}——执行SQL】\r\n{UtilMethods.GetNativeSql(sql, pars)}\r\n";
                       var originColor = Console.ForegroundColor;
                       if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                           Console.ForegroundColor = ConsoleColor.Green;
                       if (sql.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase) || sql.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
                           Console.ForegroundColor = ConsoleColor.Yellow;
                       if (sql.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
                           Console.ForegroundColor = ConsoleColor.Red;
                       Console.WriteLine(log);
                       Console.ForegroundColor = originColor;
                       Console.Write(log);
                       //App.PrintToMiniProfiler("SqlSugar", "Info", log);
                   };
                   db.Aop.OnError = ex =>
                   {
                       if (ex.Parametres == null) return;
                       var log = $"【{DateTime.Now}——错误SQL】\r\n{UtilMethods.GetNativeSql(ex.Sql, (SugarParameter[])ex.Parametres)}\r\n";

                       Console.ForegroundColor = ConsoleColor.Red;
                       Console.Write(log);
                       // Log.Error(log, ex);
                       //App.PrintToMiniProfiler("SqlSugar", "Error", log);
                   };
                   db.Aop.OnLogExecuted = (sql, pars) =>
                   {
                       //// 若参数值超过100个字符则进行截取
                       //foreach (var par in pars)
                       //{
                       //    if (par.DbType != System.Data.DbType.String || par.Value == null) continue;
                       //    if (par.Value.ToString().Length > 100)
                       //        par.Value = string.Concat(par.Value.ToString()[..100], "......");
                       //}
                       // 执行时间超过5秒时
                       if (db.Ado.SqlExecutionTime.TotalSeconds > 5)
                       {
                           var fileName = db.Ado.SqlStackTrace.FirstFileName; // 文件名
                           var fileLine = db.Ado.SqlStackTrace.FirstLine; // 行号
                           var firstMethodName = db.Ado.SqlStackTrace.FirstMethodName; // 方法名
                           var log = $"【{DateTime.Now}——超时SQL】\r\n【所在文件名】：{fileName}\r\n【代码行数】：{fileLine}\r\n【方法名】：{firstMethodName}\r\n" + $"【SQL语句】：{UtilMethods.GetNativeSql(sql, pars)}";
                           Console.ForegroundColor = ConsoleColor.Yellow;
                           Console.Write(log);
                           //Log.Warning(log);
                           //App.PrintToMiniProfiler("SqlSugar", "Slow", log);
                       }
                   };
                   // 数据审计
                   db.Aop.DataExecuting = (oldValue, entityInfo) =>
                   {
                       // 新增/插入
                       if (entityInfo.OperationType == DataFilterType.InsertByObject)
                       {
                           // 若主键是长整型且空则赋值雪花Id
                           if (entityInfo.EntityColumnInfo.IsPrimarykey && !entityInfo.EntityColumnInfo.IsIdentity && entityInfo.EntityColumnInfo.PropertyInfo.PropertyType == typeof(long))
                           {
                               var id = entityInfo.EntityColumnInfo.PropertyInfo.GetValue(entityInfo.EntityValue);
                               if (id == null || (long)id == 0)
                                   entityInfo.SetValue(0L);
                           }
                           // 若创建时间为空则赋值当前时间
                           else if (entityInfo.PropertyName == nameof(EntityBase.CreateTime) && entityInfo.EntityColumnInfo.PropertyInfo.GetValue(entityInfo.EntityValue) == null)
                           {
                               entityInfo.SetValue(DateTime.Now);
                           }
                           // 若当前用户非空（web线程时）
                           if (_accesssor.Value?.HttpContext?.User != null)
                           {
                               dynamic entityValue = entityInfo.EntityValue;

                               if (entityInfo.PropertyName == nameof(EntityBase.CreatedbyId))
                               {
                                   var createUserId = entityValue.CreatedbyId;
                                   //if (createUserId == 0 || createUserId == null)
                                   if (string.IsNullOrWhiteSpace(createUserId))
                                       entityInfo.SetValue(_accesssor.Value?.HttpContext?.User?.FindFirst(ClaimAttributes.UserId));
                               }
                               else if (entityInfo.PropertyName == nameof(EntityBase.CreatedbyName))
                               {
                                   var createUserName = entityValue.CreatedbyName;
                                   if (string.IsNullOrWhiteSpace(createUserName))
                                       entityInfo.SetValue(_accesssor.Value?.HttpContext?.User?.FindFirst(ClaimAttributes.UserName));
                               }
                           }
                       }
                       // 编辑/更新
                       else if (entityInfo.OperationType == DataFilterType.UpdateByObject)
                       {
                           if (entityInfo.PropertyName == nameof(EntityBase.LastModifiedTime))
                               entityInfo.SetValue(DateTime.Now);
                           else if (entityInfo.PropertyName == nameof(EntityBase.LastModifiedbyId))
                           {
                               var _lastModifiedbyId = _accesssor.Value?.HttpContext?.User?.FindFirst(ClaimAttributes.UserId);
                               if (_lastModifiedbyId is not null)
                               {
                                   entityInfo.SetValue(_lastModifiedbyId);
                               }
                           }
                           else if (entityInfo.PropertyName == nameof(EntityBase.LastModifiedbyName))
                           {
                               var _lastModifiedbyName = _accesssor.Value?.HttpContext?.User?.FindFirst(ClaimAttributes.UserName);
                               if (_lastModifiedbyName is not null)
                                   entityInfo.SetValue(_lastModifiedbyName);
                           }
                       }
                   };
               });
                return sqlSugar;
            });
            context.Services.AddScoped(typeof(ISimpleClient<>), typeof(SimpleClient<>)); // 仓储注册


            var LogDb = configuration.GetSection("LogDb").Get<MongoDbConfig>();
            // 注册 IMongoClient 服务
           context.Services.AddSingleton<IMongoClient>(s => new MongoClient(LogDb!.ConnectionString));
            // 注册 IMongoDatabase 服务
            context.Services.AddSingleton<IMongoDatabase>(s =>
            {
                var client = s.GetRequiredService<IMongoClient>();
                return client.GetDatabase(LogDb!.DatabaseName);
            });

          
            return base.ConfigureServicesAsync(context);
          
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddHttpContextAccessor();

           
            return base.PreConfigureServicesAsync(context);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
        {

            var app = context.GetApplicationBuilder();
            var hnagfireConfig = context.GetConfiguration().GetSection("HangfireConfig").Get<HangfireConfig>();
            if (hnagfireConfig is not null && (hnagfireConfig?.ServerEnable ?? false) && (hnagfireConfig?.DashboardIsEnable ?? false))
            {
                app.UseHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[]  { new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                        {
                            SslRedirect = false,
                            RequireSsl = false,
                            LoginCaseSensitive = true,
                            Users = new[]
                            {
                                new BasicAuthAuthorizationUser
                                {
                                    Login ="hangfire",
                                    PasswordClear ="1"
                                }
                            },
                        })
                    },
                    IgnoreAntiforgeryToken = true,//这里一定要写true 不然用client库写代码添加webjob会出错
                    AppPath = "/swagger/",//返回站点地址
                    DisplayStorageConnectionString = true,//是否显示数据库连接信息
                    IsReadOnlyFunc = Context => false,
                    //Authorization = new[] { new CustomAuthorizeFilter() }
                });
            }
            var scope = context.ServiceProvider.CreateScope();
            CaiXin.Infrastructure.Job.RecurringJobWorkRegister.RegisterRecurringJobWorks(scope);
            return base.OnPostApplicationInitializationAsync(context);
        }

         
    }
}
