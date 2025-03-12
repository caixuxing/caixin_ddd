using Microsoft.AspNetCore.ResponseCompression;

namespace CaiXin.Host
{
    /// <summary>
    /// 
    /// </summary>
    static class Program
    {
        /// <summary>
        /// 程序主入口
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        async static Task Main(string[] args)
        {
            try
            {
                // 设置Serilog
                var builder = WebApplication.CreateBuilder(args);
                builder.Host.AddAppSettingsSecretsJson().UseAutofac();
                //builder.Services.AddResponseCompression(opt =>
                //{
                //    opt.Providers.Add<GzipCompressionProvider>();
                //    opt.EnableForHttps = true;

                //});
                builder.WebHost.ConfigureKestrel((context, options) =>
                {
                    options.Limits.MaxRequestBodySize = 31457280;
                    //options.Limits.MaxConcurrentConnections = 2;
                }).UseKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = (long)(2 * Math.Pow(1024, 3));
                });
                builder.Services.ReplaceConfiguration(builder.Configuration);
                await builder.AddApplicationAsync<CaiXinHostModule>();
                var app = builder.Build();
                await app.InitializeApplicationAsync();
                app.MapControllers();
                await app.RunAsync();

            }
            catch (Exception ex)
            {

                Console.WriteLine($"服务启动失败！{ex.Message}");
            }
        }
    }
}