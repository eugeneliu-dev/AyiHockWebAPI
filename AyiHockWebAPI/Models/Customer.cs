using System;
using System.Collections.Generic;

#nullable disable

namespace AyiHockWebAPI.Models
{
    public partial class Customer
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool Enable { get; set; }
        public int Role { get; set; }
        public bool Isblack { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
        public int Money { get; set; }
        public Guid Modifier { get; set; }
        public int Platform { get; set; }
        public string PrePassword { get; set; }
    }
}
