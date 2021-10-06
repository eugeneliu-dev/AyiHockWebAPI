using AyiHockWebAPI.Dtos;
using AyiHockWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Services
{
    public class NewsService
    {
        private readonly d5qp1l4f2lmt76Context _ayihockDbContext;
        public NewsService(d5qp1l4f2lmt76Context ayihockDbContext)
        {
            _ayihockDbContext = ayihockDbContext;
        }

        public News GetNewsFullInfoFromDB(int id)
        {
            var news = (from a in _ayihockDbContext.News
                        where a.NewsId == id
                        select a).SingleOrDefault();
            return news;
        }

        public Manager GetManagerInfo(string sub)
        {
            return _ayihockDbContext.Managers.Where(mgr => sub == mgr.Email).Select(mgr => mgr).FirstOrDefault();
        }

        public async Task<List<NewsGetDto>> GetNewsList()
        {
            var newses = await (from a in _ayihockDbContext.News
                                select new NewsGetDto
                                {
                                    NewsId = a.NewsId,
                                    Title = a.Title,
                                    Content = a.Content,
                                    ManagerName = _ayihockDbContext.Managers.Where(mgr => mgr.ManagerId == a.Manager).Select(mgr => mgr.Name).SingleOrDefault(),
                                    CreateTime = a.CreateTime,
                                    ModifyTime = a.ModifyTime,
                                    IsHot = a.IsHot,
                                    CategoryName = _ayihockDbContext.Newscategories.Where(cate => cate.NewsCategoryId == a.Category).Select(cate => cate.Name).SingleOrDefault()
                                }).ToListAsync();
            return newses;
        }

        public async Task<NewsGetDto> GetNews(int id)
        {
            var newsById = await (from a in _ayihockDbContext.News
                                  where a.NewsId == id
                                  select new NewsGetDto
                                  {
                                      NewsId = a.NewsId,
                                      Title = a.Title,
                                      Content = a.Content,
                                      ManagerName = _ayihockDbContext.Managers.Where(mgr => mgr.ManagerId == a.Manager).Select(mgr => mgr.Name).SingleOrDefault(),
                                      CreateTime = a.CreateTime,
                                      ModifyTime = a.ModifyTime,
                                      IsHot = a.IsHot,
                                      CategoryName = _ayihockDbContext.Newscategories.Where(cate => cate.NewsCategoryId == a.Category).Select(cate => cate.Name).SingleOrDefault()
                                  }).SingleOrDefaultAsync();
            return newsById;
        }

        public async Task PostNews(Manager manager, NewsPostDto value)
        {
            News news = new News
            {
                Title = value.Title,
                Content = value.Content,
                Manager = manager.ManagerId,
                IsHot = value.IsHot,
                Category = value.CategoryId,
                CreateTime = DateTime.Now,
                ModifyTime = DateTime.Now
            };

            _ayihockDbContext.News.Add(news);
            await _ayihockDbContext.SaveChangesAsync();
        }

        public async Task PutNews(int id, Guid managerId, News update, NewsPutDto value)
        {
            update.NewsId = id;
            update.Title = value.Title;
            update.Content = value.Content;
            update.Manager = managerId;
            update.IsHot = value.IsHot;
            update.Category = value.CategoryId;
            update.ModifyTime = DateTime.Now;

            await _ayihockDbContext.SaveChangesAsync();
        }

        public async Task<News> DeleteNews(int id)
        {
            var delete = GetNewsFullInfoFromDB(id);
            if (delete == null)
                return null;

            _ayihockDbContext.News.Remove(delete);
            await _ayihockDbContext.SaveChangesAsync();
            return delete;
        }



    }
}
