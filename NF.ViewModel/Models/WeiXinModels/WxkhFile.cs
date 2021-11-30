using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NF.ViewModel
{
    public class WxkhFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string CategoryName { get; set;}
        /// <summary>
        /// 提醒时间
        /// </summary>
        public DateTime? TxDate { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>

        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }




    }
}
