using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Json;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using CaiXin.Application.Contracts;
using CaiXin.Application;
using Microsoft.OpenApi.Models;
using NUglify.Helpers;
using Swashbuckle.AspNetCore.SwaggerUI;
using CaiXin.Host.Filter;
using FluentValidation.AspNetCore;
using FluentValidation;
using System.Reflection;

namespace CaiXin.Host
{
   



    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpAspNetCoreMvcModule),
        typeof(CaiXinApplicationContractsModule),
        typeof(CaiXinApplicationModule)
        )]
    internal class CaiXinHostModule : AbpModule
    {

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {



            context.Services.AddFluentValidationClientsideAdapters();
            context.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            context.Services.AddControllersWithViews(opt =>
            {
                // 禁用自动模型验证
                opt.ModelValidatorProviders.Clear();
                opt.Filters.Add(typeof(ResultExceptionFilter));
                opt.Filters.Add(typeof(SysRequestLogFilter));
                opt.EnableEndpointRouting = false;
            }).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);


      
            context.Services.Configure<AbpJsonOptions>(options =>
            {
                options.InputDateTimeFormats = new List<string>() { "yyyy-MM-dd HH:mm:ss" };
            });

            //关闭系统自带的模型验证过滤器
            context.Services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);
            context.Services.AddControllers()
                .AddJsonOptions(options =>
                {

                    //数据格式首字母小写
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    //数据格式原样输出
                    // options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    //取消Unicode编码
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                    //忽略空值
                    // options.JsonSerializerOptions.IgnoreNullValues = true;
                    //允许额外符号
                    options.JsonSerializerOptions.AllowTrailingCommas = true;
                    //反序列化过程中属性名称是否使用不区分大小写的比较
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                    //options.JsonSerializerOptions.Converters.Add(new DecimalPrecisionConverter());
                    //options.JsonSerializerOptions.Converters.Add(new ObjectIdJsonConverter());


                });
            context.Services.AddEndpointsApiExplorer();
            context.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "后端管理API", Version = "v1" });

                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                //{
                //    Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.ApiKey,
                //    BearerFormat = "JWT",
                //    Scheme = "Bearer"
                //});
                ////添加安全要求
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                //    {
                //        new OpenApiSecurityScheme{
                //            Reference =new OpenApiReference{
                //                Type = ReferenceType.SecurityScheme,
                //                Id ="Bearer"
                //            }
                //        },new string[]{ }
                //    }});

                //加载xml文档注释
                Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "CaiXin.*.xml")
                 .Select(x => x)
                 .ForEach(item => c.IncludeXmlComments(item, true));

                // 开启加权小锁
                //c.OperationFilter<AddResponseHeadersFilter>();
                //c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
            });
            context.Services.AddHttpClient();
            return base.ConfigureServicesAsync(context);
        }

        public override Task OnPreApplicationInitializationAsync(ApplicationInitializationContext context)
        {
         
            return base.OnPreApplicationInitializationAsync(context);
        }


        public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStore API");
                options.DefaultModelsExpandDepth(-1); // -1 表示完全隐藏 Schemas 区域
                //options.DefaultModelExpandDepth(0);  // 可选：设置单个模型默认折叠
                options.DocExpansion(DocExpansion.None); // 可选：禁用文档中的默认展开
            });

            return base.OnApplicationInitializationAsync(context);
        }


    }
}
