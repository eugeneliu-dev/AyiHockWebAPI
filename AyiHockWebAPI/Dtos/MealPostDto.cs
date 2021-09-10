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
    public class MealPostDto
    {
        [ModelBinder(BinderType = typeof(JsonBinder))]
        [Required]
        public MealDto Meal { get; set; }
        [Required]
        public IFormFile File { get; set; }
    }
}
