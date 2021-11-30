using System;
using System.Collections.Generic;

#nullable disable

namespace NF.Model.Models
{
    public partial class CompDescription
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public string Item { get; set; }
        public string ContentText { get; set; }
        public int CreateUserId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public byte IsDelete { get; set; }
        public DateTime? TxDate { get; set; }

        public virtual UserInfor CreateUser { get; set; }
        public virtual UserInfor ModifyUser { get; set; }
    }
}
