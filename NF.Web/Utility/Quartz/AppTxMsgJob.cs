using NF.Common.Models;
using NF.Common.Utility;
using Quartz;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NF.Web.Utility.Quartz
{
    public class AppTxMsgJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            AppTxTask();

            return Task.CompletedTask;
        }
        /// <summary>
        /// 指定某个时间提醒当前需要审批条数
        /// 比如周5或者其他时候
        /// </summary>
        public void AppTxTask()
        {
            try
            {
                string dqhturl = $"{AppsetsInfo.MsgReqBaseURL}/WeiXin/DaoQi/AppRows";
                Log4netHelper.Info("指定时间提醒待处理条数url:" + dqhturl);
                WebClient client = new WebClient();
                string desc = client.DownloadString(dqhturl);
            }
            catch (Exception ex)
            {

                Log4netHelper.Error("指定时间提醒条数:" + ex.Message);
            }


        }
    }
}
