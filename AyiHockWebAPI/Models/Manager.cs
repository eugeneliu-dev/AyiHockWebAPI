using System;
using System.Collections.Generic;

#nullable disable

namespace AyiHockWebAPI.Models
{
    public partial class Manager
    {
        public Manager()
        {
            News = new HashSet<News>();
        }

        public Guid ManagerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool Enable { get; set; }
        public bool IsAdmin { get; set; }

        public virtual ICollection<News> News { get; set; }
    }
}
