using AyiHockWebAPI.Models;
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
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly d5qp1l4f2lmt76Context _ayihockDbContext;

        public CustomerController(d5qp1l4f2lmt76Context ayihockDbContext)
        {
            _ayihockDbContext = ayihockDbContext;
        }

        //[HttpGet]
        //[Authorize]
        //public ActionResult<List<OrderGetDto>> Get()
        //{
        //    var sub = User.Identity.Name;
        //    var customer = _ayihockDbContext.Customers.Select(cus => cus).Where(cus => sub == cus.Email).FirstOrDefault();

        //    if (customer == null)
        //        return BadRequest();

        //    var orderlist = (from order in _ayihockDbContext.Orders
        //                     join cus in _ayihockDbContext.Customers on order.Orderer equals cus.CustomerId
        //                     where order.Orderer == customer.CustomerId
        //                     select new OrderGetDto
        //                     {
        //                         OrderId = order.OrderId,
        //                         Status = order.Status,
        //                         TotalPrice = order.TotalPrice,
        //                         OrdererPhone = order.OrdererPhone,
        //                         CreateTime = order.CreateTime,
        //                         ModifyTime = order.ModifyTime,
        //                         OrderContents = (from content in _ayihockDbContext.Ordercontents
        //                                          join meal in _ayihockDbContext.Meals on content.MealId equals meal.MealId
        //                                          where order.OrderId == content.OrderId
        //                                          select new OrderContentGetDto
        //                                          {
        //                                              OrderId = content.OrderId,
        //                                              MealId = content.MealId,
        //                                              Quantity = content.Quantity,
        //                                              MealName = meal.Name,
        //                                              MealDesc = meal.Description,
        //                                              MealPrice = meal.Price,
        //                                              MealPic = meal.Picture
        //                                          }).ToList()
        //                     }).ToList().OrderBy(a => a.OrderId);

        //    if (orderlist == null || orderlist.Count() <= 0)
        //        return NotFound();
        //    else
        //        return Ok(orderlist);
        //}
    }
}
