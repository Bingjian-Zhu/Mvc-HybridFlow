using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceAPI.Entities
{
    public class User
    {
        [Key]
        [MaxLength(32)]
        public string UserId { get; set; }

        [MaxLength(32)]
        public string UserName { get; set; }

        [MaxLength(50)]
        public string Password { get; set; }

        public bool IsActive { get; set; }//是否可用

        public virtual ICollection<Claims> Claims { get; set; }

    }
}
