﻿using System;
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
        public string CreateTime { get; set; }
        public string ModifyTime { get; set; }
        public bool IsHot { get; set; }
        public string CategoryName { get; set; }
    }
}
