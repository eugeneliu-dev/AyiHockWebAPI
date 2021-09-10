using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Dtos
{
    public class OrderGetDto
    {
        public string OrderId { get; set; }
        public int Status { get; set; }
        public int TotalPrice { get; set; }
        public string OrdererPhone { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
        public ICollection<OrderContentGetDto> OrderContents { get; set; }
    }
}
