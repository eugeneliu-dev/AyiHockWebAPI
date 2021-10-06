using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Filters;
using AyiHockWebAPI.Helpers;
using AyiHockWebAPI.Models;
using AyiHockWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(ResultFormatFilter))]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly EncryptDecryptHelper _encryptDecryptHelper;
        private readonly CustomerService _customerService;

        public CustomerController(EncryptDecryptHelper encryptDecryptHelper,
                                  CustomerService customerService)
        {
            _encryptDecryptHelper = encryptDecryptHelper;
            _customerService = customerService;
        }

        /// <summary>
        /// 查詢使用者列表(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult<List<CustomerGetDto>>> Get()
        {
            var customers = await _customerService.GetCustomerList();

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
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult<CustomerGetDto>> Get(Guid id)
        {
            var customerById = await _customerService.GetCustomerFromManager(id);

            if (customerById == null)
                return NotFound();
            else
                return Ok(customerById);
        }

        /// <summary>
        /// 查詢使用者(僅提供一般帳號)(ApplyRole: user)
        /// </summary>
        /// <returns></returns>
        [HttpGet("user")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "normal, golden, platinum, diamond")]
        public async Task<ActionResult<CustomerGetByUserDto>> GetUser()
        {
            var sub = User.Identity.Name;

            var customer = await _customerService.GetCustomerFromUser(sub);

            if (customer == null)
                return NotFound();
            else
                return Ok(customer);
        }

        /// <summary>
        /// 新增使用者(註冊帳戶)(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Post([FromBody] CustomerPostDto value)
        {
            var getCusByEmail = await _customerService.GetCustomerFromUser(value.Email);
            if (getCusByEmail != null)
                return BadRequest();
     
            await _customerService.PostCustomer(value);

            return Ok();
        }

        /// <summary>
        /// 驗證使用者(驗證帳戶)(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [HttpGet("auth")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAuth(string varify)
        {
            //check Guid is exist?
            var customer = await _customerService.AuthCustomer(varify);
            if (customer == null)
            {
                return NotFound("驗證失敗");
            }
            else
            {
                return Ok();
            }
        }

        /// <summary>
        /// 更新使用者(僅允許更新系統資料)(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpPut("manager/{id}")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> Put(Guid id, [FromBody] CustomerPutDto value)
        {
            var manager = _customerService.GetManagerInfo(User.Identity.Name);
            if (manager == null)
                return BadRequest();

            var update = _customerService.GetCustomerFullInfoById(id);

            if (update != null)
            {
                await _customerService.PutCustomerFromManager(manager.ManagerId, value, update);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 更新使用者(僅允許更新使用者基本資料)(ApplyRole: user)
        /// </summary>
        /// <returns></returns>
        [HttpPut("user")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "normal, golden, platinum, diamond")]
        public async Task<ActionResult> Put([FromBody] CustomerPutByUserDto value)
        {
            var update = _customerService.GetCustomerFullInfoByMail(User.Identity.Name);

            if (update != null)
            {
                await _customerService.PutCustomerFromUser(value, update);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 更新使用者密碼(僅提供一般帳號)(ApplyRole: user)
        /// </summary>
        /// <returns></returns>
        [HttpPut("user/pwdmodify")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "normal, golden, platinum, diamond")]
        public async Task<ActionResult> PutPwd([FromBody] CustomerPutPwdByUserDto value)
        {
            var update = _customerService.GetCustomerFullInfoByOldPassword(User.Identity.Name, value.OldPassword);

            if (update != null)
            {
                await _customerService.PutCustomerNewPassword(value.NewPassword, update);
                return Ok();
            }
            else
            {
                return BadRequest("舊帳號輸入錯誤!");
            }
        }

        /// <summary>
        /// 忘記密碼(發送驗證碼至信箱供重置密碼)(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [HttpPut("user/pwdforget")]
        [AllowAnonymous]
        public async Task<ActionResult> PutPwdForget(CustomerPutPwdByForgetDto value)
        {
            var update = _customerService.GetCustomerFullInfoByMail(value.UserAccountMail);
            if (update != null)
            {
                await _customerService.PutCustomerResetPassword(value.UserAccountMail, update);
                return Ok();
            }
            else
            {
                return BadRequest("電子郵件輸入錯誤!");
            }
        }

        /// <summary>
        /// 重置使用者密碼(ApplyRole: anonymous)
        /// </summary>
        /// <returns></returns>
        [HttpPut("user/pwdreset")]
        [AllowAnonymous]
        public async Task<ActionResult> PutPwdReset([FromBody] CustomerPutPwdByResetDto value)
        {
            var update = _customerService.GetCustomerFullInfoByPrePassword(value.UserAccountMail, value.DefaultPassword);
            if (update != null)
            {
                await _customerService.PutCustomerNewPassword(value.NewPassword, update);
                return Ok();
            }
            else
            {
                return BadRequest("驗證碼或郵件信箱輸入錯誤!");
            }
        }

        /// <summary>
        /// 刪除使用者(ApplyRole: admin/staff)
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var delete = await _customerService.DeleteCustomer(id);

            if (delete == null)
                return NotFound("CustomerId不存在!");
            else
                return Ok();
        }


    }
}
