using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Filters;
using AyiHockWebAPI.Helpers;
using AyiHockWebAPI.Models;
using AyiHockWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(ResultFormatFilter))]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly ManagerService _managerService;

        public ManagerController(ManagerService managerService)
        {
            _managerService = managerService;
        }

        /// <summary>
        /// 查詢管理人員列表(完整資料)(ApplyRole: admin)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<ManagerGetTotalInfoDto>>> GetManagersTotalInfo()
        {
            var managers = await _managerService.GetManagersListTotalInfo();

            if (managers == null || managers.Count() <= 0)
                return NotFound();
            else
                return Ok(managers);
        }

        /// <summary>
        /// 查詢管理人員(完整資料)(ApplyRole: admin)
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ManagerGetTotalInfoDto>> GetManagerTotalInfo(Guid id)
        {
            var managerById = await _managerService.GetManagerTotalInfo(id);

            if (managerById == null)
                return NotFound();
            else
                return Ok(managerById);
        }

        /// <summary>
        /// 查詢管理人員(基本資料)(ApplyRole: staff/admin)
        /// </summary>
        /// <returns></returns>
        [HttpGet("basicinfo")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult<ManagerGetBasicInfoDto>> GetUserBasicInfo()
        {
            var sub = User.Identity.Name;

            var managerById = await _managerService.GetManagerBasicInfo(sub);

            if (managerById == null)
                return NotFound();
            else
                return Ok(managerById);
        }

        /// <summary>
        /// 新增管理人員(ApplyRole: admin)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Post([FromBody] ManagerPostDto value)
        {
            var manager = _managerService.GetManagerFullInfoByMail(value.Email);
            if (manager != null)
                return BadRequest();

            await _managerService.PostManager(value);

            return Ok();
        }

        /// <summary>
        /// 修改管理人員(ApplyRole: admin)
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Put(Guid id, [FromBody] ManagerPutDto value)
        {
            var manager = _managerService.GetManagerFullInfoByMail(value.Email);
            if (manager != null)
                return BadRequest();

            var update = _managerService.GetManagerFullInfoById(id);

            if (update != null)
            {
                await _managerService.PutManager(update, value);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 修改管理人員個人密碼(ApplyRole: staff/admin)
        /// </summary>
        /// <returns></returns>
        [HttpPut("pwd")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> PutPwd([FromBody] ManagerPutPwdDto value)
        {
            var update = _managerService.GetManagerFullInfoByOldPassword(User.Identity.Name, value.OldPassword);

            if (update != null)
            {
                await _managerService.PutManagerNewPassword(value, update);
                return Ok();
            }
            else
            {
                return BadRequest("舊帳號輸入錯誤!");
            }
        }

        /// <summary>
        /// 重置管理人員個人密碼(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [HttpPut("pwdreset")]
        [AllowAnonymous]
        public async Task<ActionResult> PutPwdReset(string mail)
        {
            var update = _managerService.GetManagerFullInfoByMail(mail);

            if (update != null)
            {
                await _managerService.PutManagerResetPassword(mail, update);
                return Ok();
            }
            else
            {
                return BadRequest("電子郵件輸入錯誤!");
            }
        }

        /// <summary>
        /// 刪除管理人員(ApplyRole: admin)
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var delete = await _managerService.DeleteManager(id);

            if (delete == null)
                return NotFound("CustomerId不存在!");
            else
                return Ok();
        }

    }
}
