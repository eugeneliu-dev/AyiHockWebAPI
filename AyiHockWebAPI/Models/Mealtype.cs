using System;
using System.Collections.Generic;

#nullable disable

namespace AyiHockWebAPI.Models
{
    public partial class Mealtype
    {
        public Mealtype()
        {
            Meals = new HashSet<Meal>();
        }

        public int TypeId { get; set; }
        public string Type { get; set; }

        public virtual ICollection<Meal> Meals { get; set; }
    }
}
