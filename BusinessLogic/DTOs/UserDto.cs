using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }
}
