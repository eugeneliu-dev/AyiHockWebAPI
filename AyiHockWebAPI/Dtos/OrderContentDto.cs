using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Dtos
{
    public class OrderContentGetDto
    {
        public string OrderId { get; set; }
        public int MealId { get; set; }
        public int Quantity { get; set; }

        public string MealName { get; set; }
        public string MealDesc { get; set; }
        public int MealPrice { get; set; }
        public string MealPic { get; set; }
    }

    public class OrderContentPostDto
    {
        public int MealId { get; set; }
        public int Quantity { get; set; }
    }
}
