using NF.Common.Models;
using NF.Common.Utility;
using Quartz;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NF.Web.Utility.Quartz
{
    public class WxZdDqJob:IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            GetAppDqoQi();

            return Task.CompletedTask;
        }

        /// <summary>
        /// 提醒周东
        /// </summary>
        public void GetAppDqoQi()
        {
            try
            {
                string dqhturl = $"{AppsetsInfo.MsgReqBaseURL}/WeiXin/DaoQi/WxZhouDongTx";
                NF.Common.Utility.Log4netHelper.Info("周东到期提醒:" + dqhturl);
                WebClient client = new WebClient();
                string desc = client.DownloadString(dqhturl);
            }
            catch (Exception ex)
            {
                Log4netHelper.Error("周东到期提醒:" + ex.Message);

            }


        }
    }
}
