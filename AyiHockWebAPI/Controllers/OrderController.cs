using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Models;
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
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly d5qp1l4f2lmt76Context _ayihockDbContext;

        public OrderController(d5qp1l4f2lmt76Context ayihockDbContext)
        {
            _ayihockDbContext = ayihockDbContext;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<List<OrderGetDto>> Get()
        {
            var sub = User.Identity.Name;
            var customer = _ayihockDbContext.Customers.Select(cus => cus).Where(cus => sub == cus.Email).FirstOrDefault();

            if (customer == null)
                return BadRequest();

            var orderlist = (from order in _ayihockDbContext.Orders
                             join cus in _ayihockDbContext.Customers on order.Orderer equals cus.CustomerId
                             where order.Orderer == customer.CustomerId
                             select new OrderGetDto
                             {
                                 OrderId = order.OrderId,
                                 Status = order.Status,
                                 TotalPrice = order.TotalPrice,
                                 OrdererPhone = order.OrdererPhone,
                                 CreateTime = order.CreateTime,
                                 ModifyTime = order.ModifyTime,
                                 OrderContents = (from content in _ayihockDbContext.Ordercontents
                                                  join meal in _ayihockDbContext.Meals on content.MealId equals meal.MealId
                                                  where order.OrderId == content.OrderId
                                                  select new OrderContentGetDto
                                                  {
                                                      OrderId = content.OrderId,
                                                      MealId = content.MealId,
                                                      Quantity = content.Quantity,
                                                      MealName = meal.Name,
                                                      MealDesc = meal.Description,
                                                      MealPrice = meal.Price,
                                                      MealPic = meal.Picture
                                                  }).ToList()
                             }).ToList().OrderBy(a => a.OrderId);

            if (orderlist == null || orderlist.Count() <= 0)
                return NotFound();
            else
                return Ok(orderlist);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Post([FromBody] OrderPostDto value)
        {
            var sub = User.Identity.Name;
            var customer = _ayihockDbContext.Customers.Select(cus => cus).Where(cus => sub == cus.Email).FirstOrDefault();

            if (customer == null)
                return BadRequest();

            Order order = new Order
            {
                OrderId = DateTime.Now.ToString("yyyyMMddHHmmssffff"),
                Status = value.Status,
                TotalPrice = value.TotalPrice,
                Payrule = value.PayRule,
                Orderer = customer.CustomerId,
                OrdererPhone = value.OrdererPhone,
                CreateTime = DateTime.Now,
                ModifyTime = DateTime.Now,
                Ordercontents = TransToModel(value.OrderContents)
            };

            _ayihockDbContext.Orders.Add(order);
            _ayihockDbContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var delete_content = (from a in _ayihockDbContext.Ordercontents
                                  where a.OrderId == id
                                  select a).ToList();

            _ayihockDbContext.Ordercontents.RemoveRange(delete_content);
            _ayihockDbContext.SaveChanges();


            var delete_order = (from a in _ayihockDbContext.Orders
                                where a.OrderId == id
                                select a).SingleOrDefault();

            if (delete_order != null)
            {
                _ayihockDbContext.Orders.Remove(delete_order);
                _ayihockDbContext.SaveChanges();
            }

            return NoContent();
        }

        private static List<Ordercontent> TransToModel(ICollection<OrderContentPostDto> orderContents)
        {
            List<Ordercontent> contents = new List<Ordercontent>(); 

            foreach (var item in orderContents)
            {
                Ordercontent content = new Ordercontent
                {
                    MealId = item.MealId,
                    Quantity = item.Quantity
                };

                contents.Add(content);
            }

            return contents;
        }
    }
}
