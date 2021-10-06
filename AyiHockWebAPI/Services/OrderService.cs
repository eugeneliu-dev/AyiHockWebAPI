using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Services
{
    public class OrderService
    {
        private readonly d5qp1l4f2lmt76Context _ayihockDbContext;
        public OrderService(d5qp1l4f2lmt76Context ayihockDbContext)
        {
            _ayihockDbContext = ayihockDbContext;
        }

        //public News GetNewsFullInfoFromDB(int id)
        //{
        //    var news = (from a in _ayihockDbContext.News
        //                where a.NewsId == id
        //                select a).SingleOrDefault();
        //    return news;
        //}

        public Customer GetCustomerInfo(string sub, string platform)
        {
            return _ayihockDbContext.Customers.Select(cus => cus).Where(cus => sub == cus.Email && cus.Platform == Convert.ToInt32(platform)).FirstOrDefault();
        }

        public async Task<List<OrderGetDto>> GetOrderList(Customer customer)
        {
            var orderlist = await (from order in _ayihockDbContext.Orders
                                   where order.Orderer == customer.CustomerId
                                   join cus in _ayihockDbContext.Customers on order.Orderer equals cus.CustomerId
                                   select new OrderGetDto
                                   {
                                       OrderId = order.OrderId,
                                       Status = order.Status,
                                       TotalPrice = order.TotalPrice,
                                       OrdererPhone = order.OrdererPhone,
                                       CreateTime = order.CreateTime,
                                       ModifyTime = order.ModifyTime,
                                       OrderContents = (from content in _ayihockDbContext.Ordercontents
                                                        where order.OrderId == content.OrderId
                                                        join meal in _ayihockDbContext.Meals on content.MealId equals meal.MealId
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
                                   }).OrderBy(a => a.OrderId).ToListAsync();
            return orderlist;
        }

        public async Task PostOrder(Customer customer, OrderPostDto value)
        {
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

            int money = customer.Money + value.TotalPrice;
            int type = (int)GetUserNewType(money);

            customer.Money = value.TotalPrice;
            if (type != customer.Role)
                customer.Role = type;

            await _ayihockDbContext.SaveChangesAsync();
        }

        public async Task DeleteOrder(string id)
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
                await _ayihockDbContext.SaveChangesAsync();
            }
        }


        private List<Ordercontent> TransToModel(ICollection<OrderContentPostDto> orderContents)
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

        private CustomerType GetUserNewType(int currMoney)
        {
            if (currMoney >= 50000)
            {
                return CustomerType.Diamond;
            }
            else if (currMoney >= 10000)
            {
                return CustomerType.Platinum;
            }
            else if (currMoney >= 5000)
            {
                return CustomerType.Golden;
            }
            else
            {
                return CustomerType.Normal;
            }
        }

        private enum CustomerType
        {
            Normal = 0,
            Golden = 1,
            Platinum = 2,
            Diamond = 3
        }



    }
}
