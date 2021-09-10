using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace AyiHockWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealController : ControllerBase
    {
        private readonly d5qp1l4f2lmt76Context _ayihockDbContext;
        private readonly IWebHostEnvironment _env;
        public MealController(d5qp1l4f2lmt76Context ayihockDbContext, IWebHostEnvironment env)
        {
            _ayihockDbContext = ayihockDbContext;
            _env = env;
        }

        [HttpGet]
        public ActionResult<List<MealGetDto>> Get()
        {
            var meals = (from a in _ayihockDbContext.Meals
                         select new MealGetDto
                         {
                             MealId = a.MealId,
                             Name = a.Name,
                             Description = a.Description,
                             Type = a.Type,
                             Price = a.Price,
                             PicPath = a.Picture
                         }).ToList();

            if (meals == null || meals.Count() <= 0)
                return NotFound();
            else
                return Ok(meals);
        }

        [HttpGet("types")]
        public ActionResult<List<Mealtype>> GetTypes()
        {
            var types = (from a in _ayihockDbContext.Mealtypes
                         select new Mealtype
                         {
                             TypeId = a.TypeId,
                             Type = a.Type
                         }).ToList().OrderBy(a => a.TypeId);

            if (types == null || types.Count() <= 0)
                return NotFound();
            else
                return Ok(types);
        }

        [HttpGet("{id}")]
        public ActionResult<MealGetDto> Get(int id)
        {
            var mealById = (from a in _ayihockDbContext.Meals
                            where a.MealId == id
                            select new MealGetDto
                            {
                                MealId = a.MealId,
                                Name = a.Name,
                                Description = a.Description,
                                Type = a.Type,
                                Price = a.Price,
                                PicPath = a.Picture
                            }).SingleOrDefault();

            if (mealById == null)
                return NotFound();
            else
                return mealById;
        }

        [HttpGet("typeid/{type_id}")]
        public ActionResult<List<MealGetDto>> GetByTypeId(int type_id)
        {
            var res = (from a in _ayihockDbContext.Mealtypes
                       select a).ToList().OrderBy(a => a.TypeId);

            if (type_id >= res.First().TypeId || type_id <= res.Last().TypeId)
            {
                var mealsByType = (from a in _ayihockDbContext.Meals
                                   where a.Type == type_id
                                   select new MealGetDto
                                   {
                                       MealId = a.MealId,
                                       Name = a.Name,
                                       Description = a.Description,
                                       Type = a.Type,
                                       Price = a.Price,
                                       PicPath = a.Picture
                                   }).ToList().OrderBy(a => a.MealId);

                if (mealsByType == null || mealsByType.Count() <= 0)
                    return NotFound();
                else
                    return Ok(mealsByType);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public ActionResult Post([FromForm] MealPostDto value)
        {
            string root = _env.ContentRootPath + @"\wwwroot\meals\";
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            string filePath = root + value.File.FileName;

            try
            {
                using (var stream = System.IO.File.Create(filePath))
                {
                    value.File.CopyTo(stream);
                }

                Meal meal = new Meal
                {
                    Name = value.Meal.Name,
                    Description = value.Meal.Description,
                    Type = value.Meal.Type,
                    Price = value.Meal.Price,
                    Picture = filePath
                };

                _ayihockDbContext.Meals.Add(meal);
            }
            catch (Exception ex)
            {
                //TODO: exception handle
                return BadRequest(ex.ToString());
            }

            _ayihockDbContext.SaveChanges();

            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] MealPutDto value)
        {
            var update = (from a in _ayihockDbContext.Meals
                          where a.MealId == id
                          select a).SingleOrDefault();

            if (update != null)
            {
                //update.Name = value.Name;
                //update.Type = value.Type;
                //update.Price = value.Price;
                //update.Description = value.Description;
                //update.Picture = value.PicPath;

                _ayihockDbContext.Meals.Update(update).CurrentValues.SetValues(value);
                _ayihockDbContext.SaveChanges();
            }
            else
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult Patch(int id, [FromBody] JsonPatchDocument value)
        {
            var update = (from a in _ayihockDbContext.Meals
                          where a.MealId == id
                          select a).SingleOrDefault();

            if (update != null)
            {
                value.ApplyTo(update);
                _ayihockDbContext.SaveChanges();
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var delete = (from a in _ayihockDbContext.Meals
                          where a.MealId == id
                          select a).SingleOrDefault();

            if (delete == null)
                return NotFound();

            _ayihockDbContext.Meals.Remove(delete);
            _ayihockDbContext.SaveChanges();

            return NoContent();
        }
    }
}
