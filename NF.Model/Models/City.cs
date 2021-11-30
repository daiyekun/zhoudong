using System;
using System.Collections.Generic;

#nullable disable

namespace NF.Model.Models
{
    public partial class City
    {
        public int Id { get; set; }
        public int? ProvinceId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public byte? IsEnable { get; set; }
    }
}
