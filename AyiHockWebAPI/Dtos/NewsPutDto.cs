using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Dtos
{
    public class NewsPutDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsHot { get; set; }
        public int CategoryId { get; set; }
    }
}
