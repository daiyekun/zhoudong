using System;
using System.Collections.Generic;

#nullable disable

namespace NF.Model.Models
{
    public partial class ContConsult
    {
        public int Id { get; set; }
        public int? ContId { get; set; }
        public int? UserId { get; set; }
    }
}
