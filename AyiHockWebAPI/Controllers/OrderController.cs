using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Filters;
using AyiHockWebAPI.Models;
using AyiHockWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(ResultFormatFilter))]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// 查詢個人訂單列表(ApplyRole: customer)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "normal, golden, platinum, diamond")]
        public async Task<ActionResult<List<OrderGetDto>>> Get()
        {
            var customer = _orderService.GetCustomerInfo(User.Identity.Name);
            if (customer == null)
                return BadRequest();

            var orderlist = await _orderService.GetOrderList(customer);

            if (orderlist == null || orderlist.Count() <= 0)
                return NotFound();
            else
                return Ok(orderlist);
        }

        /// <summary>
        /// 新增個人訂單(ApplyRole: customer)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "normal, golden, platinum, diamond")]
        public async Task<ActionResult> Post([FromBody] OrderPostDto value)
        {
            var customer = _orderService.GetCustomerInfo(User.Identity.Name);
            if (customer == null)
                return BadRequest();

            await _orderService.PostOrder(customer, value);

            return NoContent();
        }

        /// <summary>
        /// 刪除個人訂單(ApplyRole: staff/admin)
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize("JtiRestraint")]
        [Authorize(Roles = "admin, staff")]
        public async Task <ActionResult> Delete(string id)
        {
            await _orderService.DeleteOrder(id);

            return NoContent();
        }

        


        
    }
}
