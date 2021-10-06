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

    public class LoginDtoForSocial
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public int Role { get; set; }
        public bool Enable { get; set; }
        public bool IsBlack { get; set; }
    }

    public class SocialUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }

    public enum LoginPlatform
    {
        Original = 0,
        Google = 1,
        Facebook = 2
    }

}
