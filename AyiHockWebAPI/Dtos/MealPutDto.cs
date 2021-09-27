using AyiHockWebAPI.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Dtos
{
    public class MealPutBasicInfoDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Type { get; set; }
    }

    public class MealPutTotalInfoDto
    {
        [ModelBinder(BinderType = typeof(JsonBinder))]
        [Required]
        public MealDto Meal { get; set; }
        [Required]
        public IFormFile File { get; set; }
    }
}
