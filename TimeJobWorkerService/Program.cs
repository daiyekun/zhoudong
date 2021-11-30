

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeJobWorkerService.Utility;

namespace TimeJobWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration Configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                    GetDevQuartConfig(Configuration);
                    services.AddHostedService<WxFwhWorker>();
                });
        /// <summary>
        /// ∂¡»°¥Ê¥¢≈‰÷√
        /// </summary>
        /// <param name="Configuration"></param>
        private static void GetDevQuartConfig(IConfiguration Configuration)
        {
            DevQuartzConnModels.QuartzUrl = Configuration.GetSection("WxQuartzConn:QuartzUrl").Value;
            DevQuartzConnModels.QuartzWxCron = Configuration.GetSection("WxQuartzConn:QuartzWxCron").Value;
        }
    }
}

