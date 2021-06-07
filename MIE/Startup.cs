using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MIE.Dao;
using MIE.Dao.Impl;
using MIE.Utils;

namespace MIE
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // 使用该方法添加服务到容器中
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<MySQLDbContext>(option =>
            {
                option.UseMySQL(Configuration.GetConnectionString("MySqlConnection"));
            });

            services
              .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidIssuer = Configuration["Jwt:Issuer"],
                      ValidAudience = Configuration["Jwt:Audience"],
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"])),
                      ClockSkew = TimeSpan.Zero
                  };
              });

            // 依赖注入
            services.AddScoped<IUserDao, UserDaoImpl>();
            services.AddScoped<IBlogDao, BlogDaoImpl>();
            services.AddScoped<IAuthUtil, AuthUtil>();
            services.AddScoped<IReservationDao, ReservationDaoImpl>();
            services.AddScoped<IAvailableTimeDao, AvailableTimeDaoImpl>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        }

        // 使用该方法配置Http请求管道
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
