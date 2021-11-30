using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NF.ViewModel.Models
{
    /// <summary>
    /// 金蝶推送视图
    /// </summary>
   public class VOUCHERPUSHCONTRACT
    {/// <summary>
     /// 付款类型， 0：收款，1：付款
     /// </summary>
        public int FinceType { get; set; }
        /// <summary>
        /// 合同编号
        /// </summary>
        public string ContractNo { get; set; }
        /// <summary>
        /// 状态，默认为已确认
        /// </summary>
        public int Astate { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal AmountMoney { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public int Currency { get; set; }

        /// <summary>
        /// 结算方式
        /// </summary>
        public int SettlementMethod { get; set; }
        /// <summary>
        /// 实际结算时间
        /// </summary>
        public DateTime ActualSettlementDate { get; set; }
        /// <summary>
        /// 票据号码
        /// </summary>
        public string VoucherNo { get; set; }
        /// <summary>
        /// 推送凭证
        /// </summary>
        public string TuisongPz { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 建立人
        /// </summary>
        public int CreateUser { get; set; }
        /// <summary>
        /// 建立时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 确认人
        /// </summary>
        public int ConfirmUserId { get; set; }
        /// <summary>
        /// 确认时间
        /// </summary>
        public DateTime ConfirmDateTIme { get; set; }



    }
}
