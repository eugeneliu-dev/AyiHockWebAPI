using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Helpers;
using AyiHockWebAPI.Models;
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
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly d5qp1l4f2lmt76Context _ayihockDbContext;
        private readonly IConfiguration _configuration;
        private readonly EncryptDecryptHelper _encryptDecryptHelper;
        private readonly AutoSendEmailHelper _autoSendEmailHelper;

        public ManagerController(d5qp1l4f2lmt76Context ayihockDbContext,
                                  IConfiguration configuration,
                                  EncryptDecryptHelper encryptDecryptHelper,
                                  AutoSendEmailHelper autoSendEmailHelper)
        {
            _ayihockDbContext = ayihockDbContext;
            _configuration = configuration;
            _encryptDecryptHelper = encryptDecryptHelper;
            _autoSendEmailHelper = autoSendEmailHelper;
        }

        /// <summary>
        /// 查詢管理人員列表(完整資料)(ApplyRole: admin)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult<List<ManagerGetTotalInfoDto>> GetUsersTotalInfo()
        {
            var managers = (from a in _ayihockDbContext.Managers
                            where a.IsAdmin == false
                            select new ManagerGetTotalInfoDto
                            {
                                ManagerId = a.ManagerId,
                                Name = a.Name,
                                Email = a.Email,
                                Phone = a.Phone,
                                Enable = a.Enable,
                                IsAdmin = a.IsAdmin
                            }).ToList().OrderBy(a => a.Enable);

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
        public ActionResult<ManagerGetTotalInfoDto> GetUserTotalInfo(Guid id)
        {
            var managerById = (from a in _ayihockDbContext.Managers
                                where a.IsAdmin == false
                                select new ManagerGetTotalInfoDto
                                {
                                    ManagerId = a.ManagerId,
                                    Name = a.Name,
                                    Email = a.Email,
                                    Phone = a.Phone,
                                    Enable = a.Enable,
                                    IsAdmin = a.IsAdmin
                                }).SingleOrDefault();

            if (managerById == null)
                return NotFound();
            else
                return managerById;
        }

        /// <summary>
        /// 查詢管理人員(基本資料)(ApplyRole: staff/admin)
        /// </summary>
        /// <returns></returns>
        [HttpGet("basicinfo")] 
        [Authorize]
        public ActionResult<ManagerGetBasicInfoDto> GetUserBasicInfo()
        {
            var sub = User.Identity.Name;

            var managerById = (from a in _ayihockDbContext.Managers
                               where a.Email == sub
                               select new ManagerGetBasicInfoDto
                               {
                                   Name = a.Name,
                                   Email = a.Email,
                                   Phone = a.Phone,
                                   IsAdmin = a.IsAdmin
                               }).SingleOrDefault();

            if (managerById == null)
                return NotFound();
            else
                return managerById;
        }

        /// <summary>
        /// 新增管理人員(ApplyRole: admin)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Post([FromBody] ManagerPostDto value)
        {
            //var sub = User.Identity.Name;
            var getMgrByEmail = (from a in _ayihockDbContext.Managers
                                 where a.Email == value.Email
                                 select a).SingleOrDefault();
            if (getMgrByEmail != null)
                return BadRequest();

            Guid guid = Guid.NewGuid();

            Manager manager = new Manager
            {
                ManagerId = guid,
                Name = value.Name,
                Email = value.Email,
                Password = _encryptDecryptHelper.AESDecrypt(value.Password), //需前端加密處理,後端解密
                Phone = value.Phone,
                Enable = true,
                IsAdmin = false
            };

            _ayihockDbContext.Managers.Add(manager);
            _ayihockDbContext.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// 修改管理人員(ApplyRole: admin)
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public ActionResult Put(Guid id, [FromBody] ManagerPutDto value)
        {
            var getMgrByEmail = (from a in _ayihockDbContext.Managers
                                 where a.Email == value.Email
                                 select a).SingleOrDefault();
            if (getMgrByEmail != null)
                return BadRequest();

            var update = (from a in _ayihockDbContext.Managers
                          where a.ManagerId == id 
                          select a).SingleOrDefault();

            if (update != null)
            {
                update.Name = value.Name;
                update.Email = value.Email;
                update.Phone = value.Phone;
                update.Enable = value.Enable;

                _ayihockDbContext.SaveChanges();
            }
            else
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// 修改管理人員個人密碼(ApplyRole: staff/admin)
        /// </summary>
        /// <returns></returns>
        [HttpPut("userpwd")]
        public ActionResult PutPwd([FromBody] ManagerPutPwdDto value)
        {
            var sub = User.Identity.Name;

            var update = (from a in _ayihockDbContext.Managers
                          where a.Email == sub && a.Password == _encryptDecryptHelper.AESDecrypt(value.OldPassword)
                          select a).SingleOrDefault();

            if (update != null)
            {
                update.Password = _encryptDecryptHelper.AESDecrypt(value.NewPassword);  //需前端加密處理,後端解密

                _ayihockDbContext.SaveChanges();
            }
            else
            {
                return BadRequest();
            }

            return NoContent();
        }

        /// <summary>
        /// 重置管理人員個人密碼(ApplyRole: staff/admin)
        /// </summary>
        /// <returns></returns>
        [HttpPut("userpwdreset")]
        public ActionResult PutPwdReset()
        {
            var sub = User.Identity.Name;

            var update = (from a in _ayihockDbContext.Managers
                          where a.Email == sub
                          select a).SingleOrDefault();

            if (update != null)
            {
                string newPwd = _encryptDecryptHelper.GetRandomStr();

                update.Password = newPwd;
                _ayihockDbContext.SaveChanges();

                _autoSendEmailHelper.SendAuthEmail(sub, "新密碼如下:\n" + newPwd);
            }
            else
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// 刪除管理人員(ApplyRole: admin)
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var delete = (from a in _ayihockDbContext.Managers
                          where a.ManagerId == id
                          select a).SingleOrDefault();

            if (delete == null)
                return NotFound();

            _ayihockDbContext.Managers.Remove(delete);
            _ayihockDbContext.SaveChanges();

            return NoContent();
        }

    }
}
