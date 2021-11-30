using System;
using System.Collections.Generic;

#nullable disable

namespace NF.Model.Models
{
    public partial class RoleModule
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
    }
}
