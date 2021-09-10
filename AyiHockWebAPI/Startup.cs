using AyiHockWebAPI.Helpers;
using AyiHockWebAPI.Models;
using AyiHockWebAPI.Services;
using AyiHockWebAPI.ValidationAttributes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static AyiHockWebAPI.ValidationAttributes.ValidJtiRequirement;

namespace AyiHockWebAPI
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
            //注入Helper
            services.AddSingleton<JwtHelper>();

            //注入Service
            services.AddScoped<LoginService>();

            //PostgreSQL Connection
            services.AddDbContext<d5qp1l4f2lmt76Context>(options => options.UseNpgsql(Configuration.GetConnectionString("AyihockDB")));

            //自訂Cors
            services.AddCors(option =>
            {
                //Policy名稱CorsPolicy是自訂的，可以自己改
                option.AddPolicy("CorsPolicy", policy =>
                {
                    //設定允許跨域的來源，有多個的話可以用`,`隔開
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            services.AddControllers();
            
            //SwaggerDoc
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "AyiHockWebAPI", Version = "v1" }); });


            //JWT驗證
            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy("JtiRestraint", policy =>
                    {
                        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                        policy.RequireAuthenticatedUser();
                        policy.Requirements.Add(new ValidJtiRequirement());
                    });
                })
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
                    options.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // 透過這項宣告，就可以從 "sub" 取值並設定給 User.Identity.Name
                        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                        // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
                        //RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/roles",
                        RoleClaimType = ClaimTypes.Role,

                        // 一般我們都會驗證 Issuer
                        ValidateIssuer = true,
                        ValidIssuer = Configuration.GetValue<string>("JwtSettings:Issuer"),

                        // 通常不太需要驗證 Audience
                        ValidateAudience = false,
                        //ValidAudience = "JwtAuthDemo", // 不驗證就不需要填寫

                        // 一般我們都會驗證 Token 的有效期間
                        ValidateLifetime = true,

                        // 如果 Token 中包含 key 才需要驗證，一般都只有簽章而已
                        ValidateIssuerSigningKey = true,

                        // "1234567890123456" (16個字元) 並應從 IConfiguration 取得
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JwtSettings:SignKey")))
                    };
                });

            services.AddTransient<IAuthorizationHandler, ValidJtiHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AyiHockWebAPI v1"));
            }

            //套用Policy到Middleware
            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            //先驗證，再授權
            app.UseAuthentication();
            app.UseAuthorization();

            //啟用靜態目錄
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
