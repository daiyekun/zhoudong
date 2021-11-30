using NF.Common.Models;
using NF.Common.Utility;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NF.Web.Utility.Quartz
{
    /// <summary>
    /// 审批超时审批再次发送
    /// </summary>
    public class AppDaoQiMsg : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            GetAppDqoQi();

            return Task.CompletedTask;
        }

        /// <summary>
        /// 超时两条到期2天
        /// </summary>
        public void GetAppDqoQi()
        {
            try
            {
                string dqhturl = $"{AppsetsInfo.MsgReqBaseURL}/WeiXin/DaoQi/AppDaoQi";
                Log4netHelper.Info("超时2天未审批url:" + dqhturl);
                WebClient client = new WebClient();
                string desc = client.DownloadString(dqhturl);
            }
            catch (Exception ex)
            {
                Log4netHelper.Error("超时两天未发送:" + ex.Message);

            }


        }

    }
}
