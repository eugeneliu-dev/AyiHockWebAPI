using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Services
{
    public class MealService
    {
        private readonly d5qp1l4f2lmt76Context _ayihockDbContext;
        private readonly IWebHostEnvironment _env;

        public MealService(d5qp1l4f2lmt76Context ayihockDbContext, IWebHostEnvironment env)
        {
            _ayihockDbContext = ayihockDbContext;
            _env = env;
        }

        public Meal GetMealFullInfoFromDB(int id)
        {
            var meal = (from a in _ayihockDbContext.Meals
                        where a.MealId == id
                        select a).SingleOrDefault();
            return meal;
        }

        public async Task<List<MealGetDto>> GetMealList()
        {
            var meals = await (from a in _ayihockDbContext.Meals
                               select new MealGetDto
                               {
                                   MealId = a.MealId,
                                   Name = a.Name,
                                   Description = a.Description,
                                   Type = a.Type,
                                   Price = a.Price,
                                   PicPath = a.Picture
                               }).ToListAsync();
            return meals;
        }

        public async Task<List<Mealtype>> GetMealTypes()
        {
            var types = await (from a in _ayihockDbContext.Mealtypes
                               select new Mealtype
                               {
                                   TypeId = a.TypeId,
                                   Type = a.Type
                               }).OrderBy(a => a.TypeId).ToListAsync();
            return types;
        }

        public async Task<MealGetDto> GetMeal(int id)
        {
            var mealById = await (from a in _ayihockDbContext.Meals
                                  where a.MealId == id
                                  select new MealGetDto
                                  {
                                      MealId = a.MealId,
                                      Name = a.Name,
                                      Description = a.Description,
                                      Type = a.Type,
                                      Price = a.Price,
                                      PicPath = a.Picture
                                  }).SingleOrDefaultAsync();
            return mealById;
        }

        public async Task<List<MealGetDto>> GetMealListByTypeId(int type_id)
        {
            var mealsByType = await (from a in _ayihockDbContext.Meals
                                     where a.Type == type_id
                                     select new MealGetDto
                                     {
                                         MealId = a.MealId,
                                         Name = a.Name,
                                         Description = a.Description,
                                         Type = a.Type,
                                         Price = a.Price,
                                         PicPath = a.Picture
                                     }).OrderBy(a => a.MealId).ToListAsync();
            return mealsByType;
        }

        public async Task PostMeal(MealPostDto value)
        {
            string root = _env.ContentRootPath + @"\wwwroot\meals\";
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            string filePath = root + value.File.FileName;

            var ss = new List<string>();
            var dd = ss[0];

            using (var stream = System.IO.File.Create(filePath))
            {
                await value.File.CopyToAsync(stream);
            }

            Meal meal = new Meal
            {
                Name = value.Meal.Name,
                Description = value.Meal.Description,
                Type = value.Meal.Type,
                Price = value.Meal.Price,
                Picture = @"\wwwroot\meals\" + value.File.FileName
            };

            _ayihockDbContext.Meals.Add(meal);
            await _ayihockDbContext.SaveChangesAsync();
        }











    }
}
