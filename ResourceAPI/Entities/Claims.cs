using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceAPI.Entities
{
    public class Claims
    {
        [MaxLength(32)]
        public int ClaimsId { get; set; }

        [MaxLength(32)]
        public string Type { get; set; }

        [MaxLength(32)]
        public string Value { get; set; }

        public virtual User User { get; set; }

    }
}
