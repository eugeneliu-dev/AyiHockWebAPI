using System;
using System.Collections.Generic;

#nullable disable

namespace AyiHockWebAPI.Models
{
    public partial class Jtiblacklist
    {
        public string Jti { get; set; }
        public DateTime Expire { get; set; }
    }
}
