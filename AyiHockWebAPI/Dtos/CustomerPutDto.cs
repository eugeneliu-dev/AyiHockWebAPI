using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Dtos
{
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

    public class CustomerPutDto
    {
        public Guid CustomerId { get; set; }
        public bool Enable { get; set; }
        public int Role { get; set; }
        public bool Isblack { get; set; }
        public DateTime ModifyTime { get; set; }
    }
}
