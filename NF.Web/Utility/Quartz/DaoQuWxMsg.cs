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
    public class DaoQuWxMsg : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            GetDqhtWxMsgToSubmit();
            GetDqJhWxMsgToSubmit();

            return Task.CompletedTask;
        }
        /// <summary>
        /// 到期合同获取微信消息
        /// </summary>
        public void GetDqhtWxMsgToSubmit()
        {
            try
            {
                string dqhturl = $"{AppsetsInfo.MsgReqBaseURL}/WeiXin/DaoQi/DaoQiHt";
                Log4netHelper.Info("到期合同定时url:" + dqhturl);
                WebClient client = new WebClient();
                string desc = client.DownloadString(dqhturl);
            }
            catch (Exception ex)
            {
                Log4netHelper.Error("定时合同到期异常:" + ex.Message);

            }


        }

        /// <summary>
        /// 到期计划资金获取微信消息
        /// </summary>
        public void GetDqJhWxMsgToSubmit()
        {
            try
            {
                string dqhturl = $"{AppsetsInfo.MsgReqBaseURL}/WeiXin/DaoQi/DaoQiJh";
                Log4netHelper.Info("到期计划定时url:" + dqhturl);
                WebClient client = new WebClient();
                string desc = client.DownloadString(dqhturl);
            }
            catch (Exception ex)
            {

                Log4netHelper.Error("定时计划资金到期异常:" + ex.Message);
            }


        }
    }
}

