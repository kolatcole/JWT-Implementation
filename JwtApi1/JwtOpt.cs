using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtApi1
{
    public class JwtOpt
    {
        public string Secret { get; set; }

        public string ExpiryTime { get; set; }
    }
}
