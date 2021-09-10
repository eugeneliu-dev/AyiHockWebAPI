using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Dtos
{
    public class OrderContentPostDto
    {
        public int MealId { get; set; }
        public int Quantity { get; set; }
    }
}
