using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs
{
    public class UserWithRolesDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public ICollection<string> Roles { get; set; }
    }
}
