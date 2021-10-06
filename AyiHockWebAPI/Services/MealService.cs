using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Interface;
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
        private readonly ICloudStorage _cloudStorage;

        public MealService(d5qp1l4f2lmt76Context ayihockDbContext, IWebHostEnvironment env, ICloudStorage cloudStorage)
        {
            _ayihockDbContext = ayihockDbContext;
            _env = env;
            _cloudStorage = cloudStorage;
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
                               where a.Disable == false
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

        public async Task<List<MealTypeGetDto>> GetMealTypes()
        {
            var types = await (from a in _ayihockDbContext.Mealtypes
                               select new MealTypeGetDto
                               {
                                   TypeId = a.TypeId,
                                   Type = a.Type
                               }).OrderBy(a => a.TypeId).ToListAsync();
            return types;
        }

        public async Task<MealGetDto> GetMeal(int id)
        {
            var mealById = await (from a in _ayihockDbContext.Meals
                                  where a.MealId == id && a.Disable == false
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
                                     where a.Type == type_id && a.Disable == false
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
            var cloudImgPath = await _cloudStorage.UploadFileAsync(value.File, value.File.FileName);

            Meal meal = new Meal
            {
                Name = value.Meal.Name,
                Description = value.Meal.Description,
                Type = value.Meal.Type,
                Price = value.Meal.Price,
                Picture = cloudImgPath,
                Picturename = value.File.FileName
            };

            _ayihockDbContext.Meals.Add(meal);
            await _ayihockDbContext.SaveChangesAsync();
        }

        public async Task PutMeal(Meal update, MealPutBasicInfoDto value)
        {
            _ayihockDbContext.Meals.Update(update).CurrentValues.SetValues(value);
            await _ayihockDbContext.SaveChangesAsync();
        }

        public async Task PutMealAll(Meal update, MealPutTotalInfoDto value)
        {
            var cloudImgPath = await _cloudStorage.UploadFileAsync(value.File, value.File.FileName);
            var oldImgName = update.Picturename;

            if (update.Picturename == value.File.FileName)
            {
                _ayihockDbContext.Meals.Update(update).CurrentValues.SetValues(value.Meal);
                await _ayihockDbContext.SaveChangesAsync();
            }
            else
            {
                update.Name = value.Meal.Name;
                update.Price = value.Meal.Price;
                update.Type = value.Meal.Type;
                update.Description = value.Meal.Description;
                update.Picture = cloudImgPath;
                update.Picturename = value.File.FileName;

                await _ayihockDbContext.SaveChangesAsync();
                await _cloudStorage.DeleteFileAsync(oldImgName);
            }
        }

        public async Task<Meal> DisableMeal(int id)
        {
            var disableMeal = GetMealFullInfoFromDB(id);
            if (disableMeal == null)
                return null;

            disableMeal.Disable = true;
            await _ayihockDbContext.SaveChangesAsync();

            return disableMeal;
        }











    }
}
