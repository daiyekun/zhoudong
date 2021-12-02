using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NF.ViewModel.Models
{

    /// <summary>
    /// 审批通知条数
    /// </summary>
    public class WxTongZhiInfo
    {
        /// <summary>
        /// 微信账号
        /// </summary>
        public string WxCode { get; set; }
        /// <summary>
        /// 提醒条数
        /// </summary>
        public int Rows { get; set; }
    }
    /// <summary>
    /// 微信提醒通知
    /// </summary>
    public class WxTxTongZhi
    {
        /// <summary>
        /// 客户ID
        /// </summary>

        public int Id { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        //密码
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 微信账号
        /// </summary>

        public string WxCode { get; set; }
    }

}
