using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TimeJobWorkerService.Utility
{
    class WxGetFwhUserJobs : IJob
    {
        /// <summary>
        /// 拉取关注账号列表
        /// </summary>
            private string url = $"{DevQuartzConnModels.QuartzUrl}/Finance/JdOracle/Gtr";
            public Task Execute(IJobExecutionContext context)
            {

                WebClient client = new WebClient();
                string desc = client.DownloadString(url);
                return Task.CompletedTask;
            }
        
    }
}
