using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Filters;
using AyiHockWebAPI.Models;
using AyiHockWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace AyiHockWebAPI.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(ResultFormatFilter))]
    [ApiController]
    public class MealController : ControllerBase
    {
        private readonly d5qp1l4f2lmt76Context _ayihockDbContext;
        private readonly MealService _mealService;

        public MealController(d5qp1l4f2lmt76Context ayihockDbContext, MealService mealService)
        {
            _ayihockDbContext = ayihockDbContext;
            _mealService = mealService;
        }

        /// <summary>
        /// 查詢菜單列表(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<MealGetDto>>> Get()
        {
            var meals = await _mealService.GetMealList();

            if (meals == null || meals.Count() <= 0)
                return NotFound();
            else
                return Ok(meals);
        }

        /// <summary>
        /// 查詢菜單種類(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [HttpGet("types")]
        public async Task<ActionResult<List<Mealtype>>> GetTypes()
        {
            var types = await _mealService.GetMealTypes();

            if (types == null || types.Count() <= 0)
                return NotFound();
            else
                return Ok(types);
        }

        /// <summary>
        /// 查詢單一菜色(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MealGetDto>> Get(int id)
        {
            var mealById = await _mealService.GetMeal(id);

            if (mealById == null)
                return NotFound();
            else
                return mealById;
        }

        /// <summary>
        /// 查詢菜單列表(依菜單種類)(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [HttpGet("typeid/{type_id}")]
        public async Task<ActionResult<List<MealGetDto>>> GetByTypeId(int type_id)
        {
            var check = await (from a in _ayihockDbContext.Mealtypes
                               select a).OrderBy(a => a.TypeId).ToListAsync();

            if (type_id >= check.First().TypeId || type_id <= check.Last().TypeId)
            {
                var mealsByType = await _mealService.GetMealListByTypeId(type_id);

                if (mealsByType == null || mealsByType.Count() <= 0)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(mealsByType);
                }
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// 新增菜色(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> Post([FromForm] MealPostDto value)
        {
            await _mealService.PostMeal(value);

            return Ok();
        }

        /// <summary>
        /// 修改單一菜色(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> Put(int id, [FromBody] MealPutDto value)
        {
            var update = _mealService.GetMealFullInfoFromDB(id);

            if (update != null)
            {
                _ayihockDbContext.Meals.Update(update).CurrentValues.SetValues(value);
                await _ayihockDbContext.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// 修改單一菜色(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument value)
        {
            var update = _mealService.GetMealFullInfoFromDB(id);

            if (update != null)
            {
                value.ApplyTo(update);
                await _ayihockDbContext.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 刪除單一菜色(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> Delete(int id)
        {
            var delete = _mealService.GetMealFullInfoFromDB(id);

            if (delete == null)
                return NotFound();

            _ayihockDbContext.Meals.Remove(delete);
            await _ayihockDbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
