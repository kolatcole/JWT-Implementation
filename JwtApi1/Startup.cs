using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
namespace JwtApi1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(opt => {
                opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Test Api",
                    Version = "v1"
                });

                //First we define the security scheme
                opt.AddSecurityDefinition("Bearer", //Name the security scheme
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Type = SecuritySchemeType.Http, //We set the scheme type to http since we're using bearer authentication
                        Scheme = "bearer" //The name of the HTTP Authorization scheme to be used in the Authorization header. In this case "bearer".
                     });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement{
                     {
                        new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                        Id = "Bearer", //The name of the previously defined security scheme.
                        Type = ReferenceType.SecurityScheme
                     }
            },new List<string>()
    }
});


                //opt.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                //{
                //    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                //    Name = "Authorization",
                //    Description = "Authorization",
                //    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
                //}) ;
                //var security = new Dictionary<string, IEnumerable<string>> {
                //    { "Bearer",new string[0]}
                //};
                //opt.AddSecurityRequirement(new OpenApiSecurityRequirement());

            });
            services.AddDbContext<AppDbContext>(opt=> {
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            //        services.AddIdentity<ApplicationUser, ApplicationRole>()
            //.AddEntityFrameworkStores<MyContext, Guid>()
            //.AddUserStore<ApplicationUserStore>() //this one provides data storage for user.
            //.AddRoleStore<ApplicationRoleStore>()
            //.AddUserManager<ApplicationUserManager>()
            //.AddRoleManager<ApplicationRoleManager>()
            //.AddDefaultTokenProviders();

            services.AddIdentity<Customer, IdentityRole>(opt =>
            {


            }).AddRoles<IdentityRole>()
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders().AddUserManager<UserManager<Customer>>();
            var JwtOptSection = Configuration.GetSection(nameof(JwtOpt));
            services.Configure<JwtOpt>(JwtOptSection);

            var jwtOpt = JwtOptSection.Get<JwtOpt>();
            var expiry = jwtOpt.ExpiryTime;
            var key = jwtOpt.Secret;

           

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(opt=> {
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters { 
                    IssuerSigningKey=new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    ValidateAudience=false,
                    ValidateIssuer=false
                };
            });
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("SuperUsers", p => p.RequireClaim("SuperUser", "True"));
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();

            var swaggerOpt = new SwaggerOpt(); 
                Configuration.GetSection(nameof(SwaggerOpt)).Bind(swaggerOpt);
            app.UseSwagger(opt => {
                opt.RouteTemplate = swaggerOpt.JsonRoute;
            });
            app.UseSwaggerUI(opt => {
                opt.SwaggerEndpoint(swaggerOpt.UIEndPoint, swaggerOpt.Description);
            });
        }
       
    }
}
