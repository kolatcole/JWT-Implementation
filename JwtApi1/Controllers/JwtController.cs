using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JwtApi1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy ="SuperUsers")]
    public class JwtController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly JwtOpt jwtOpt;
        private readonly UserManager<Customer> _usermanager;
        public JwtController(AppDbContext _context, IOptions<JwtOpt> _jwtOpt,UserManager<Customer> userManager)
        {
            _usermanager = userManager;
            context = _context;
            jwtOpt = _jwtOpt.Value;
        }

        [AllowAnonymous]
        // GET: api/Jwt
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            
            var result = await context.Users.ToArrayAsync();
            return Ok(result);
        }
        
        // GET: api/Jwt/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok("value");
        }

        [AllowAnonymous]
        // POST: api/Jwt
        [HttpPost]
        public async Task<IActionResult> Create(Customer data)
        {


            try
            {
                await _usermanager.CreateAsync(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ok();
            

        }
        [AllowAnonymous]
        [HttpGet("CreateSUToken/{ID}")]
        public async Task<IActionResult> CreateSUToken(string ID)
        {
            var customer = await _usermanager.FindByIdAsync(ID);
            var claimResult = await _usermanager.AddClaimAsync(customer, new Claim("SuperUser", "True"));
            var getClaim = await _usermanager.GetClaimsAsync(customer);
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub,customer.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,customer.Id)
            }.Union(getClaim);
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("12hgdfsdghjktxrcthvjbkuycfghjhyf3"));
            if (customer != null)
            {
                var Stoken = new JwtSecurityToken(
                    issuer: "",
                    audience: "",
                    claims: claims,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOpt.Secret)), SecurityAlgorithms.HmacSha256),
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtOpt.ExpiryTime))


                    );

                //var tokenDescriptor = new SecurityTokenDescriptor
                //{
                //    Audience = "",
                //    Issuer = "",
                //    Subject = new ClaimsIdentity(new Claim[] {
                //        new Claim(JwtRegisteredClaimNames.Sub, customer.Username),
                //        new Claim(JwtRegisteredClaimNames.Jti, customer.ID.ToString())
                //    }),
                //    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
                //    Expires= DateTime.Now.AddHours(2)

                //};
                //var tokeHandler = new JwtSecurityTokenHandler();
                //var token = tokeHandler.CreateToken(tokenDescriptor);
                return Ok(new JwtSecurityTokenHandler().WriteToken(Stoken));

            }
            return Ok(customer);
        }

        [AllowAnonymous]
        [HttpGet("CreateToken/{ID}")]
        public async Task<IActionResult> CreateToken(string ID)
        {
            var customer = await _usermanager.FindByIdAsync(ID);
            //var claimResult = await _usermanager.AddClaimAsync(customer, new Claim("SuperUser", "True"));
            //var getClaim = await _usermanager.GetClaimsAsync(customer);
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub,customer.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,customer.Id)
            }/*.Union(getClaim)*/;
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("12hgdfsdghjktxrcthvjbkuycfghjhyf3"));
            if (customer != null)
            {
                var Stoken = new JwtSecurityToken(
                    issuer: "",
                    audience: "",
                    claims: claims,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOpt.Secret)), SecurityAlgorithms.HmacSha256),
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtOpt.ExpiryTime))


                    );

                //var tokenDescriptor = new SecurityTokenDescriptor
                //{
                //    Audience = "",
                //    Issuer = "",
                //    Subject = new ClaimsIdentity(new Claim[] {
                //        new Claim(JwtRegisteredClaimNames.Sub, customer.Username),
                //        new Claim(JwtRegisteredClaimNames.Jti, customer.ID.ToString())
                //    }),
                //    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
                //    Expires= DateTime.Now.AddHours(2)

                //};
                //var tokeHandler = new JwtSecurityTokenHandler();
                //var token = tokeHandler.CreateToken(tokenDescriptor);
                return Ok(new JwtSecurityTokenHandler().WriteToken(Stoken));

            }
            return Ok(customer);
        }

        
        // PUT: api/Jwt/5
        [HttpPut("{id}")]
        public ActionResult Put(int id)
        {
            return Ok("Good Policy");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
