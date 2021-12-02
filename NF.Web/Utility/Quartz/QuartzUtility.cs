using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NF.Web.Utility.Quartz
{
    public class QuartzDaoqiUtility
    {
        /// <summary>
        /// Quarzt初始化
        /// </summary>
        //public static void QuarztInit(string WxCron)
        //{
        //    IScheduler scheduler = new StdSchedulerFactory().GetScheduler().Result;
        //    IJobDetail testJob = JobBuilder.Create<DaoQuWxMsg>()
        //             .WithIdentity("dqhetong", "groupdq")
        //             .WithDescription("this is hetongtimedqoqijob")
        //             .StoreDurably()
        //             .Build();

        //    ITrigger trigger =
        //                TriggerBuilder.Create()
        //                     .StartAt(DateTime.Now)//什么时候开始执行
        //                     .WithCronSchedule(WxCron) //https://cron.qqe2.com/
        //                     .Build();
        //    //每个三秒执行一次：0/3 * * * * ?
        //    // 每天执行一次0 0 0 1/1 * ?
        //    scheduler.ScheduleJob(testJob, trigger);
        //    scheduler.Start();
        //}


        ///// <summary>
        ///// Quarzt初始化
        ///// </summary>
        //public static void QuarztInitFlow(string WxCron)
        //{
        //    IScheduler scheduler = new StdSchedulerFactory().GetScheduler().Result;
        //    IJobDetail testJob = JobBuilder.Create<AppDaoQiMsg>()
        //             .WithIdentity("dqhetongapp", "groupdqapp")
        //             .WithDescription("this is hetongtimedqoqijobapp")
        //             .StoreDurably()
        //             .Build();

        //    ITrigger trigger =
        //                TriggerBuilder.Create()
        //                     .StartAt(DateTime.Now)//什么时候开始执行
        //                     .WithCronSchedule(WxCron) //https://cron.qqe2.com/
        //                     .Build();
        //    //每个三秒执行一次：0/3 * * * * ?
        //    // 每天执行一次0 0 0 1/1 * ?
        //    scheduler.ScheduleJob(testJob, trigger);
        //    scheduler.Start();
        //}


        ///// <summary>
        ///// 指定时间提醒待处理条数
        ///// </summary>
        //public static void QuarztIniAppRows(string WxCron)
        //{
        //    IScheduler scheduler = new StdSchedulerFactory().GetScheduler().Result;
        //    IJobDetail testJob = JobBuilder.Create<AppTxMsgJob>()
        //             .WithIdentity("dqhetongapprow", "groupdqapprow")
        //             .WithDescription("this is hetongtimedqoqijobapprow")
        //             .StoreDurably()
        //             .Build();

        //    ITrigger trigger =
        //                TriggerBuilder.Create()
        //                     .StartAt(DateTime.Now)//什么时候开始执行
        //                     .WithCronSchedule(WxCron) //https://cron.qqe2.com/
        //                     .Build();
        //    //每个三秒执行一次：0/3 * * * * ?
        //    // 每天执行一次0 0 0 1/1 * ?
        //    scheduler.ScheduleJob(testJob, trigger);
        //    scheduler.Start();
        //}

        /// <summary>
        /// 到期提醒
        /// </summary>
        public static void QuarztIniDaoQi(string WxCron)
        {
            IScheduler scheduler = new StdSchedulerFactory().GetScheduler().Result;
            IJobDetail testJob = JobBuilder.Create<WxZdDqJob>()
                     .WithIdentity("dqhetongapprow", "groupdqapprow")
                     .WithDescription("this is hetongtimedqoqijobapprow")
                     .StoreDurably()
                     .Build();

            ITrigger trigger =
                        TriggerBuilder.Create()
                             .StartAt(DateTime.Now)//什么时候开始执行
                             .WithCronSchedule(WxCron) //https://cron.qqe2.com/
                             .Build();
            //每个三秒执行一次：0/3 * * * * ?
            // 每天执行一次0 0 0 1/1 * ?
            scheduler.ScheduleJob(testJob, trigger);
            scheduler.Start();
        }
    }
}
