using AuditLog.EF;
using AuditLogDemo.Authentication;
using AuditLogDemo.EF;
using AuditLogDemo.Fliters;
using AuditLogDemo.Helper;
using AuditLogDemo.Models;
using AuditLogDemo.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LogDashboard;
using System.Reflection;
using StackExchange.Profiling.Storage;

namespace AuditLogDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// autofac容器
        /// </summary>
        public ILifetimeScope AutofacContainer { get; private set; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //审计日志存储
            services.AddDbContext<AuditLogDBContent>(options =>
            {
                string conn = Configuration.GetConnectionString("LogDB");
                options.UseSqlite(conn, options =>
                {
                    options.MigrationsAssembly("AuditLogDemo");
                });
            });

            //services.AddSpaStaticFiles(configuration =>
            //{
            //    configuration.RootPath = "wwwroot/dist";
            //});
            //services.AddAutoMapper();
            //依赖注入
            //Scoped：一个请求创建一个
            //services.AddScoped<IRepository<AuditInfo>, AuditLogRepository>();
            ////每次创建一个
            //services.AddTransient<IAuditLogService, AuditLogService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthenticateService, TokenAuthenticationService>();

            //注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuditLogDemo API", Version = "v1" });
                #region 启用swagger验证功能
                //添加一个必须的全局安全信息和AddSecurityDefinition方法指定的方案名称一致即可，Bearer。
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 在下方输入Bearer {token} 即可，注意两者之间有空格",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer",

                });
                #endregion
            });


            services.AddTransient<CustomerAuthenticationHandler>();

            services.Configure<JwtSetting>(Configuration.GetSection("JWTSetting"));
            var token = Configuration.GetSection("JWTSetting").Get<JwtSetting>();
            //JWT认证
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = MultAuthenticationHandler.MultAuthName;
                x.DefaultChallengeScheme = MultAuthenticationHandler.MultAuthName;
                x.AddScheme<MultAuthenticationHandler>(MultAuthenticationHandler.MultAuthName, MultAuthenticationHandler.MultAuthName);
                x.AddScheme<CustomerAuthenticationHandler>(CustomerAuthenticationHandler.CustomerSchemeName, CustomerAuthenticationHandler.CustomerSchemeName);
            });
            //.AddJwtBearer(x =>
            //{
            //    x.RequireHttpsMetadata = false;
            //    x.SaveToken = true;
            //    x.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.SecretKey)),
            //        ValidIssuer = token.Issuer,
            //        ValidAudience = token.Audience,
            //        ValidateIssuer = false,
            //        ValidateAudience = false
            //    };
            //});

            services.AddMiniProfiler(options =>
            {
                //访问地址路由根目录；默认为：/mini-profiler-resources
                options.RouteBasePath = "/profiler";
                //数据缓存时间
                (options.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(60);
                //sql格式化设置
                options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();
                //跟踪连接打开关闭
                options.TrackConnectionOpenClose = true;
                //界面主题颜色方案;默认浅色
                options.ColorScheme = StackExchange.Profiling.ColorScheme.Dark;
                //.net core 3.0以上：对MVC过滤器进行分析
                options.EnableMvcFilterProfiling = true;
                //对视图进行分析
                options.EnableMvcViewProfiling = true;

                //控制访问页面授权，默认所有人都能访问
                //options.ResultsAuthorize;
                //要控制分析哪些请求，默认说有请求都分析
                //options.ShouldProfile;

                //内部异常处理
                //options.OnInternalError = e => MyExceptionLogger(e);
            })
            // AddEntityFramework是要监控EntityFrameworkCore生成的SQL
            .AddEntityFramework();


            services.AddLogDashboard();

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(AuditLogActionFilter));
                options.Filters.Add(typeof(ApiRequestTimeFilterAttribute));
            });

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //autofac 新增 可选
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseLogDashboard();

            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuditLogDemo API V1");
                c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("AuditLogDemo.wwwroot.index.html");
            });

            //app.UseSpaStaticFiles();

            //该方法必须在app.UseMvc以前
            app.UseMiniProfiler();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action=Index}/{id?}");
            });

            //app.UseSpa(configuration =>
            //{
            //});
        }


        /// <summary>
        /// 配置容器：在ConfigureServices后执行
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // 直接用Autofac注册我们自定义的 
            builder.RegisterModule(new AutofacModuleRegister());
        }
    }
}
