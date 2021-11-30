using Microsoft.Extensions.Configuration;
using NF.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NF.Common.Utility
{
    /// <summary>
    /// 配置文件读取
    /// </summary>
    public class AppsettingsConfigUtility
    {
        public void Initial(IConfiguration configuration)
        {

            AppsetsInfo.MsgReqBaseURL = configuration["ConnectionStrings:ReqBaseUrl"];
        }

    }
}
