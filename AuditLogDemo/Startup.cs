using AuditLog.EF;
using AuditLogDemo.EF;
using AuditLogDemo.Fliters;
using AuditLogDemo.Helper;
using AuditLogDemo.Models;
using AuditLogDemo.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
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

        //依赖注入
        //Scoped：一个请求创建一个
        //services.AddScoped<IRepository<AuditInfo>, AuditLogRepository>();
        ////每次创建一个
        //services.AddTransient<IAuditLogService, AuditLogService>();

        services.AddControllers(options =>
        {
            options.Filters.Add(typeof(AuditLogActionFilter));
        });

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

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        //autofac 新增 可选
        this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

    }
}
}
