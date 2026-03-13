using System;
using System.Collections.Generic;

#nullable disable

namespace NF.Model.Models
{
    public partial class EnterpriseInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Remark { get; set; }
        public byte? Cstate { get; set; }
        public int CreateUserId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public int? PrincipalUserId { get; set; }
    }
}
