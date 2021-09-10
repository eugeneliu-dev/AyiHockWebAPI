using System;
using System.Collections.Generic;

#nullable disable

namespace AyiHockWebAPI.Models
{
    public partial class Customertype
    {
        public int TypeId { get; set; }
        public string Name { get; set; }
        public int UpgradeLimit { get; set; }
    }
}
