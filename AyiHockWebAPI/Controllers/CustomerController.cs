using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Helpers;
using AyiHockWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly d5qp1l4f2lmt76Context _ayihockDbContext;
        private readonly IConfiguration _configuration;
        private readonly EncryptDecryptHelper _encryptDecryptHelper;
        private readonly AutoSendEmailHelper _autoSendEmailHelper;

        public CustomerController(d5qp1l4f2lmt76Context ayihockDbContext, 
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
        /// 查詢使用者列表(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult<List<CustomerGetDto>> Get()
        {
            var customers = (from a in _ayihockDbContext.Customers
                             select new CustomerGetDto
                             {
                                 CustomerId = a.CustomerId,
                                 Name = a.Name,
                                 Email = a.Email,
                                 Phone = a.Phone,
                                 Enable = a.Enable,
                                 Role = a.Role,
                                 Isblack = a.Isblack,
                                 Money = a.Money,
                                 Modifier = a.Modifier,
                                 CreateTime = a.CreateTime,
                                 ModifyTime = a.ModifyTime
                             }).ToList().OrderBy(a => a.CreateTime);

            if (customers == null || customers.Count() <= 0)
                return NotFound();
            else
                return Ok(customers);
        }

        /// <summary>
        /// 查詢使用者(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<CustomerGetDto> Get(Guid id)
        {
            var customerById = (from a in _ayihockDbContext.Customers
                                where a.CustomerId == id
                                select new CustomerGetDto
                                {
                                    CustomerId = a.CustomerId,
                                    Name = a.Name,
                                    Email = a.Email,
                                    Phone = a.Phone,
                                    Enable = a.Enable,
                                    Role = a.Role,
                                    Isblack = a.Isblack,
                                    Money = a.Money,
                                    Modifier = a.Modifier,
                                    CreateTime = a.CreateTime,
                                    ModifyTime = a.ModifyTime
                                }).SingleOrDefault();

            if (customerById == null)
                return NotFound();
            else
                return Ok(customerById);
        }

        /// <summary>
        /// 查詢使用者(ApplyRole: user)
        /// </summary>
        /// <returns></returns>
        [HttpGet("user")]
        [Authorize]
        public ActionResult<CustomerGetByUserDto> GetUser()
        {
            var sub = User.Identity.Name;

            var customer = (from a in _ayihockDbContext.Customers
                            where a.Email == sub
                            select new CustomerGetByUserDto
                            {
                                Name = a.Name,
                                Email = a.Email,
                                Phone = a.Phone,
                                Enable = a.Enable,
                                Role = a.Role,
                                ModifyTime = a.ModifyTime
                            }).SingleOrDefault();

            if (customer == null)
                return NotFound();
            else
                return customer;
        }

        /// <summary>
        /// 新增使用者(註冊帳戶)(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Post([FromBody] CustomerPostDto value)
        {
            //var sub = User.Identity.Name;
            var getCusByEmail = (from a in _ayihockDbContext.Customers
                                 where a.Email == value.Email
                                 select a).SingleOrDefault();
            if (getCusByEmail != null)
                return BadRequest();

            Guid guid = Guid.NewGuid();

            Customer customer = new Customer
            {
                CustomerId = guid,
                Name = value.Name,
                Email = value.Email,
                Password = _encryptDecryptHelper.AESDecrypt(value.Password).Replace("\"", ""), //需前端加密處理,後端解密
                Phone = value.Phone,
                Enable = false,
                Isblack = false,
                Modifier = guid,
                CreateTime = DateTime.Now,
                ModifyTime = DateTime.Now
            };

            _ayihockDbContext.Customers.Add(customer);
            _ayihockDbContext.SaveChanges();

            string authLink = "連結如下:\n" + "https://localhost:44394/api/customer/auth?varify=" + _encryptDecryptHelper.AESEncrypt(guid.ToString());
            _autoSendEmailHelper.SendAuthEmail(value.Email, authLink);

            return NoContent();
        }

        /// <summary>
        /// 驗證使用者(驗證帳戶)(ApplyRole: user)
        /// </summary>
        /// <returns></returns>
        [HttpGet("auth")]
        public ActionResult PostAuth(string varify)
        {
            //trans varifyStr to Guid
            Guid guid = Guid.Parse(_encryptDecryptHelper.AESDecrypt(varify));

            //check Guid is exist?
            var customer = (from a in _ayihockDbContext.Customers
                            where a.CustomerId == guid && a.Enable == false
                            select a).SingleOrDefault();

            if (customer == null)
            {
                return NotFound("varifyication is failed");
            }
            else
            {
                //modify Enable to 'true'
                customer.Enable = true;
                _ayihockDbContext.SaveChanges();

                string url = _configuration.GetValue<string>("WebSite:Url");
                System.Diagnostics.Process.Start("explorer", url);
            }

            return NoContent();
        }

        /// <summary>
        /// 更新使用者(僅允許更新系統資料)(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpPut("manager/{id}")]
        public ActionResult Put(Guid id, [FromBody] CustomerPutDto value)
        {
            var update = (from a in _ayihockDbContext.Customers
                          where a.CustomerId == id
                          select a).SingleOrDefault();

            if (update != null)
            {
                update.CustomerId = value.CustomerId;
                update.Enable = value.Enable;
                update.Role = value.Role;
                update.Isblack = value.Isblack;
                update.ModifyTime = value.ModifyTime;
                update.Modifier = id;

                _ayihockDbContext.SaveChanges();
            }
            else
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// 更新使用者(僅允許更新使用者基本資料)(ApplyRole: user)
        /// </summary>
        /// <returns></returns>
        [HttpPut("user")]
        public ActionResult Put([FromBody] CustomerPutByUserDto value)
        {
            var sub = User.Identity.Name;

            var update = (from a in _ayihockDbContext.Customers
                          where a.Email == sub
                          select a).SingleOrDefault();

            if (update != null)
            {
                update.Name = value.Name;
                update.Phone = value.Phone;
                update.ModifyTime = DateTime.Now;
                update.Modifier = update.CustomerId;

                _ayihockDbContext.SaveChanges();
            }
            else
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// 更新使用者密碼(ApplyRole: user)
        /// </summary>
        /// <returns></returns>
        [HttpPut("user/pwd")]
        public ActionResult PutPwd([FromBody] CustomerPutPwdByUserDto value)
        {
            var sub = User.Identity.Name;

            var update = (from a in _ayihockDbContext.Customers
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
        /// 重置使用者密碼(ApplyRole: user)
        /// </summary>
        /// <returns></returns>
        [HttpPut("user/pwdreset")]
        public ActionResult PutPwdReset()
        {
            var sub = User.Identity.Name;

            var update = (from a in _ayihockDbContext.Customers
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
        /// 刪除使用者(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var delete = (from a in _ayihockDbContext.Customers
                          where a.CustomerId == id
                          select a).SingleOrDefault();

            if (delete == null)
                return NotFound();

            _ayihockDbContext.Customers.Remove(delete);
            _ayihockDbContext.SaveChanges();

            return NoContent();
        }


    }
}
