
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeJobWorkerService.Utility;

namespace TimeJobWorkerService
{

    /// <summary>
    /// 服务号拉去定时
    /// </summary>
   public class WxFwhWorker: BackgroundService
    {
        private readonly ILogger<WxFwhWorker> _logger;
        public WxFwhWorker(ILogger<WxFwhWorker> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IScheduler scheduler = new StdSchedulerFactory().GetScheduler().Result;
            IJobDetail testJob = JobBuilder.Create<WxGetFwhUserJobs>()
                     .WithIdentity("DevTestfwh", "Devgroupfwh")
                     .WithDescription("Get Fwh Code")
                     .StoreDurably()
                     .Build();

            ITrigger trigger =
                        TriggerBuilder.Create()
                             .StartAt(DateTime.Now)//什么时候开始执行
                             .WithCronSchedule(DevQuartzConnModels.QuartzWxCron)// 时间表达式
                             .Build();

            await scheduler.ScheduleJob(testJob, trigger);
            await scheduler.Start();
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.CompletedTask;
        }
    }
}
