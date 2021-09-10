using System;
using System.Collections.Generic;

#nullable disable

namespace AyiHockWebAPI.Models
{
    public partial class News
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid Manager { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsHot { get; set; }
        public int? Category { get; set; }
        public DateTime? ModifyTime { get; set; }

        public virtual Newscategory CategoryNavigation { get; set; }
        public virtual Manager ManagerNavigation { get; set; }
    }
}
