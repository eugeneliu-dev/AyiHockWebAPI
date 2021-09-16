using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Filters;
using AyiHockWebAPI.Models;
using AyiHockWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(ResultFormatFilter))]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly NewsService _newsService;
        public NewsController(NewsService newsService)
        {
            _newsService = newsService;
        }

        /// <summary>
        /// 查詢新聞列表(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[ResultFormatFilter]
        [TypeFilter(typeof(ResultFormatFilter))]
        public async Task<ActionResult<List<NewsGetDto>>> Get()
        {
            var newses = await _newsService.GetNewsList();

            if (newses == null || newses.Count() <= 0)
                return NotFound();
            else
                return Ok(newses);
        }

        /// <summary>
        /// 查詢新聞(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<NewsGetDto>> Get(int id)
        {
            var newsById = await _newsService.GetNews(id);

            if (newsById == null)
                return NotFound();
            else
                return Ok(newsById);
        }

        /// <summary>
        /// 新增新聞(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> Post([FromBody] NewsPostDto value)
        {
            var manager = _newsService.GetManagerInfo(User.Identity.Name);
            if (manager == null)
                return BadRequest();

            await _newsService.PostNews(manager, value);

            return NoContent();
        }

        /// <summary>
        /// 修改新聞(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> Put(int id, [FromBody] NewsPutDto value)
        {
            var manager = _newsService.GetManagerInfo(User.Identity.Name);
            if (manager == null)
                return BadRequest();

            var update = _newsService.GetNewsFullInfoFromDB(id);

            if (update != null)
            {
                await _newsService.PutNews(id, manager.ManagerId, update, value);
            }
            else
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// 刪除新聞(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> Delete(int id)
        {
            await _newsService.DeleteNews(id);

            return NoContent();
        }

    }
}
