using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NF.ViewModel.Models.WeiXinModels
{
    /// <summary>
    /// 微信提交的客户实体
    /// </summary>
    public class WxCustomerInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string FirstContact { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string FirstContactMobile { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 微信账号
        /// </summary>

        public string WxCode { get; set; }


    }
}
