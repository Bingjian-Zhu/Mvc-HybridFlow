using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceAPI.Models
{
    public class Claims
    {
        public Claims(string type,string value)
        {
            Type = type;
            Value = value;
        }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
