using System;
using System.Collections.Generic;

#nullable disable

namespace AyiHockWebAPI.Models
{
    public partial class Newscategory
    {
        public Newscategory()
        {
            News = new HashSet<News>();
        }

        public int NewsCategoryId { get; set; }
        public string Name { get; set; }
        public bool IsAdminChoose { get; set; }

        public virtual ICollection<News> News { get; set; }
    }
}
