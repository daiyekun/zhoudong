using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NF.ViewModel
{
    public class CheckInfoList
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? TxDate { get; set; }
        public string CompanyName { get; set; }
    }

    public class CheckInfoView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? TxDate {  get; set; }
        public string CompanyName { get; set; }
        public string Remark {  get; set; }
    }

    public class EnterpriseInfoList
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class EnterpriseInfoView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Remark {  get; set; }
    }


    /// <summary>
    /// 微信提交的公司资料实体
    /// </summary>
    public class WxEnterpriseInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 微信账号
        /// </summary>

        public string WxCode { get; set; }
        /// <summary>
        /// 权限授权码
        /// </summary>
        public string QxCode { get; set; }


    }

    /// <summary>
    /// 微信提交的公司检测实体
    /// </summary>
    public class WxCheckInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 微信账号
        /// </summary>

        public string WxCode { get; set; }
        /// <summary>
        /// 权限授权码
        /// </summary>
        public string QxCode { get; set; }


    }

}
