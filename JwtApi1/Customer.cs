using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JwtApi1
{
    public class Customer:IdentityUser
    {
        public Customer() : base()
        {

        }

    }
}
