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

namespace AuditLogDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// autofac����
        /// </summary>
        public ILifetimeScope AutofacContainer { get; private set; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //�����־�洢
            services.AddDbContext<AuditLogDBContent>(options =>
            {
                string conn = Configuration.GetConnectionString("LogDB");
                options.UseSqlite(conn, options =>
                {
                    options.MigrationsAssembly("AuditLogDemo");
                });
            });

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot/dist";
            });
            //services.AddAutoMapper();
            //����ע��
            //Scoped��һ�����󴴽�һ��
            //services.AddScoped<IRepository<AuditInfo>, AuditLogRepository>();
            ////ÿ�δ���һ��
            //services.AddTransient<IAuditLogService, AuditLogService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthenticateService, TokenAuthenticationService>();

            //ע��Swagger������������һ���Ͷ��Swagger �ĵ�
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuditLogDemo API", Version = "v1" });
                #region ����swagger��֤����
                //���һ�������ȫ�ְ�ȫ��Ϣ��AddSecurityDefinition����ָ���ķ�������һ�¼��ɣ�Bearer��
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
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ���·�����Bearer {token} ���ɣ�ע������֮���пո�",
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer",

                });
                #endregion
            });

            services.Configure<JwtSetting>(Configuration.GetSection("JWTSetting"));
            var token = Configuration.GetSection("JWTSetting").Get<JwtSetting>();
            //JWT��֤
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.SecretKey)),
                    ValidIssuer = token.Issuer,
                    ValidAudience = token.Audience,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddLogDashboard();

            services.AddControllers(options =>
            {
                //options.Filters.Add(typeof(AuditLogActionFilter));
            });

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //autofac ���� ��ѡ
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.UseStaticFiles();


            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseLogDashboard();

            //�����м����������Swagger��ΪJSON�ս��
            app.UseSwagger();
            //�����м�������swagger-ui��ָ��Swagger JSON�ս��
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuditLogDemo API V1");
            });

            app.UseSpaStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(configuration =>
            {
            });
        }


        /// <summary>
        /// ������������ConfigureServices��ִ��
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // ֱ����Autofacע�������Զ���� 
            builder.RegisterModule(new AutofacModuleRegister());
        }
    }
}
