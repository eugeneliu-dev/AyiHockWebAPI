﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Dtos
{
    public class MealGetDto
    {
        public int MealId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Type { get; set; }
        public string PicPath { get; set; }
    }

    public class MealTypeGetDto
    {
        public int TypeId { get; set; }
        public string Type { get; set; }
    }
}
