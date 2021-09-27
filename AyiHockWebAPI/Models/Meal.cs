using System;
using System.Collections.Generic;

#nullable disable

namespace AyiHockWebAPI.Models
{
    public partial class Meal
    {
        public int MealId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Type { get; set; }
        public string Picture { get; set; }
        public string Picturename { get; set; }

        public virtual Mealtype TypeNavigation { get; set; }
    }
}
