using AyiHockWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Dtos
{
    public class OrderPostDto
    {
        public int Status { get; set; }
        public int TotalPrice { get; set; }
        public string OrdererPhone { get; set; }
        public int PayRule { get; set; }
        public ICollection<OrderContentPostDto> OrderContents { get; set; }
    }
}
