using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class UserDto
    {
        public string UserName { get; set; }

        public string Token { get; set; }

        public string  photoUrl { get; set; }

        public string KnownAs { get; set; }

        public string Gender { get; set; }
    }
}