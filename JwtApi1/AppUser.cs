using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtApi1
{
    public class AppUser:IdentityUser
    {
        public AppUser():base()
        { 
        
        }
    }
}
