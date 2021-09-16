using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Dtos
{
    public class ManagerGetTotalInfoDto
    {
        public Guid ManagerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Enable { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class ManagerGetBasicInfoDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class ManagerPostDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
    }

    public class ManagerPutDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Enable { get; set; }
    }

    public class ManagerPutPwdDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
