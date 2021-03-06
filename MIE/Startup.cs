using System;
using System.Text;
using Elasticsearch.Net;
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
using MIE.Config;
using MIE.Dao;
using MIE.Dao.Impl;
using Microsoft.Extensions.ML;
using MIE.Service;
using MIE.Service.impl;
using MIE.Utils;
using Nest;
using StackExchange.Redis;
using MIE.Entity;
using System.IO;

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
                option.UseMySQL(Configuration["ConnectionStrings:MySqlConnection"]);
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

            // 注册ElasticSearch设定
            services.Configure<ElasticSearchConfig>(Configuration.GetSection("B2bElasticSearch"));
            // 注入ElasticSearch连接池
            services.AddSingleton<IConnectionPool, B2bElasticConnectionPool>();
            // 注入ElasticSearch Client
            services.AddScoped<IElasticClient, B2bElasticClient>();
            // 注册PredictionEnginePool，线上排序预测
            //services.AddPredictionEnginePool<SubmissionDetail, QuizPrediction>()
            //    .FromFile(Path.Combine(Environment.CurrentDirectory, "MLModels", "QuizRecommenderModel.zip"));

            // 依赖注入
            services.AddScoped<IUserDao, UserDaoImpl>();
            services.AddScoped<IBlogDao, BlogDaoImpl>();
            services.AddScoped<IReservationDao, ReservationDaoImpl>();
            services.AddScoped<IAvailableTimeDao, AvailableTimeDaoImpl>();
            services.AddScoped<IQuizDao, QuizDaoImpl>();
            services.AddScoped<ISubmissionDao, SubmissionDaoImpl>();
            services.AddScoped<IAuthUtil, AuthUtil>();
            services.AddScoped<IRecommend, RecommendImpl>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Configuration["ConnectionStrings:RedisConnection"]));
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
