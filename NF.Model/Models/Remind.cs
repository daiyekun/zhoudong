using System;
using System.Collections.Generic;

#nullable disable

namespace NF.Model.Models
{
    public partial class Remind
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public string Name { get; set; }
        public string CustomName { get; set; }
        public int? AheadDays { get; set; }
        public int? DelayDays { get; set; }
        public int? IsDelete { get; set; }
    }
}
