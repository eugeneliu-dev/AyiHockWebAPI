using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Dtos
{
    public class CustomerGetDto
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Enable { get; set; }
        public int Role { get; set; }
        public bool Isblack { get; set; }
        public int Money { get; set; }
        public Guid Modifier { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
        public int Platform { get; set; }
    }

    public class CustomerGetByUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Enable { get; set; }
        public int Role { get; set; }
        public DateTime ModifyTime { get; set; }
    }

    public class CustomerPostDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
    }

    public class CustomerPutByUserDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    public class CustomerPutPwdByUserDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class CustomerPutPwdByForgetDto
    {
        public string UserAccountMail { get; set; }
    }

    public class CustomerPutPwdByResetDto
    {
        public string UserAccountMail { get; set; }
        public string DefaultPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class CustomerPutDto
    {
        public Guid CustomerId { get; set; }
        public bool Enable { get; set; }
        public int Role { get; set; }
        public bool Isblack { get; set; }
        public DateTime ModifyTime { get; set; }
    }
}
