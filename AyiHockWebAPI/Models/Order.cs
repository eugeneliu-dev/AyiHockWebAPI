using System;
using System.Collections.Generic;

#nullable disable

namespace AyiHockWebAPI.Models
{
    public partial class Order
    {
        public Order()
        {
            Ordercontents = new HashSet<Ordercontent>();
        }

        public string OrderId { get; set; }
        public int Status { get; set; }
        public int TotalPrice { get; set; }
        public Guid Orderer { get; set; }
        public string OrdererPhone { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
        public int? Payrule { get; set; }

        public virtual ICollection<Ordercontent> Ordercontents { get; set; }
    }
}
