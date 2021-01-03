using AuditLog.EF;
using AuditLogDemo.EF;
using AuditLogDemo.Fliters;
using AuditLogDemo.Models;
using AuditLogDemo.Services;
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

namespace AuditLogDemo
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
            //审计日志存储
            services.AddDbContext<AuditLogDBContent>(options =>
            {
                string conn = Configuration.GetConnectionString("LogDB");
                options.UseSqlite(conn, options =>
                {
                    options.MigrationsAssembly("AuditLog.EF");
                });
            });

            //依赖注入
            services.AddScoped<IRepository<AuditInfo>, AuditLogRepository>();
            services.AddScoped<IAuditLogService, AuditLogService>();


            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(AuditLogActionFilter));
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
