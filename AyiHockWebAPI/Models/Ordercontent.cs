using System;
using System.Collections.Generic;

#nullable disable

namespace AyiHockWebAPI.Models
{
    public partial class Ordercontent
    {
        public string OrderId { get; set; }
        public int MealId { get; set; }
        public int Quantity { get; set; }
        public Guid OrdercontentId { get; set; }

        public virtual Order Order { get; set; }
    }
}
