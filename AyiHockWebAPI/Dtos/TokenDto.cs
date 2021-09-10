using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Dtos
{
    public class TokenDto
    {
        public string Access { get; set; }
        public string Refresh { get; set; }
    }
}
