using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Dtos
{
    public class NewsGetDto
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ManagerName { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
        public bool IsHot { get; set; }
        public string CategoryName { get; set; }
    }

    public class NewsPostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsHot { get; set; }
        public int CategoryId { get; set; }
    }

    public class NewsPutDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsHot { get; set; }
        public int CategoryId { get; set; }
    }
}
