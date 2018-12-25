using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceAPI.Entities;

namespace ResourceAPI.Models
{
    public class User
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Claims> Claims { get; set; } = new HashSet<Claims>();
    }
}
