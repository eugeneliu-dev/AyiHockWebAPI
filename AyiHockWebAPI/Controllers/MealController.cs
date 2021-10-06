using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Filters;
using AyiHockWebAPI.Interface;
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
        private readonly MealService _mealService;

        public MealController(MealService mealService)
        {
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
                return Ok(mealById);
        }

        /// <summary>
        /// 查詢菜單列表(依菜單種類)(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [HttpGet("typeid/{type_id}")]
        public async Task<ActionResult<List<MealGetDto>>> GetByTypeId(int type_id)
        {
            var types = await _mealService.GetMealTypes();

            if (type_id >= types.First().TypeId || type_id <= types.Last().TypeId)
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
        [ResourceTypeFilter]
        public async Task<ActionResult> Post([FromForm] MealPostDto value)
        {
            await _mealService.PostMeal(value);

            return Ok();
        }

        /// <summary>
        /// 修改單一菜色(只修改資訊)(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpPut("basicinfo/{id}")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> Put(int id, [FromBody] MealPutBasicInfoDto value)
        {
            var update = _mealService.GetMealFullInfoFromDB(id);

            if (update != null)
            {
                await _mealService.PutMeal(update, value);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 修改單一菜色(修改資訊+圖片)(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpPut("allinfo/{id}")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> PutAll(int id, [FromForm] MealPutTotalInfoDto value)
        {
            var update = _mealService.GetMealFullInfoFromDB(id);

            if (update != null)
            {
                await _mealService.PutMealAll(update, value);
                return Ok();
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
            var delete = await _mealService.DisableMeal(id);
            
            if (delete == null)
                return NotFound("MealId不存在!");
            else
                return Ok();
        }
    }
}
