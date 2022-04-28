using System;
using System.Collections.Generic;

#nullable disable

namespace NF.Model.Models
{
    public partial class ContAttacFile
    {
        public int Id { get; set; }
        public int? AttId { get; set; }
        public string FilePath { get; set; }
        public string GuidFileName { get; set; }
        public string FolderName { get; set; }
        public int? CompanyId { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
        public string Extend { get; set; }
        public int CreateUserId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int ModifyUserId { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public byte IsDelete { get; set; }
    }
}
