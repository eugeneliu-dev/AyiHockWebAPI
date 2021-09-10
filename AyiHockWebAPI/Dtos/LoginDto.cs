using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Dtos
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginDtoWithRole
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public string Name { get; set; }
    }
}
