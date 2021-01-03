using AuditLogDemo.Models;
using AuditLogDemo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AuditLogDemo.Fliters
{
    public class AuditLogActionFilter : IAsyncActionFilter
    {
        /// <summary>
        /// 审计日志服务对象
        /// </summary>
        private readonly IAuditLogService _auditLogService;
        /// <summary>
        /// 登录用户
        /// </summary>
        private readonly ISession _Session;
        /// <summary>
        /// 日志记录
        /// </summary>
        private readonly ILogger<AuditLogActionFilter> _logger;

        public AuditLogActionFilter(
            IAuditLogService auditLogService,
            ISession Session,
            ILogger<AuditLogActionFilter> logger
            )
        {
            _Session = Session;
            _logger = logger;
            _auditLogService = auditLogService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 判断是否写日志
            if (!ShouldSaveAudit(context))
            {
                await next();
                return;
            }
            //接口Type
            var type = (context.ActionDescriptor as ControllerActionDescriptor).ControllerTypeInfo.AsType();
            //方法信息
            var method = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo;
            //方法参数
            var arguments = context.ActionArguments;
            //开始计时
            var stopwatch = Stopwatch.StartNew();
            var auditInfo = new AuditInfo
            {
                UserInfo = _Session?.Id,
                ServiceName = type != null ? type.FullName.TruncateWithPostfix(EntityDefault.FieldsLength250) : "",
                MethodName = method.Name.TruncateWithPostfix(EntityDefault.FieldsLength250),
                ////请求参数转Json
                Parameters = JsonConvert.SerializeObject(arguments),
                ExecutionTime = DateTime.Now,
                BrowserInfo = context.HttpContext.Request.Headers["User-Agent"].ToString().TruncateWithPostfix(EntityDefault.FieldsLength250),
                ClientIpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString().TruncateWithPostfix(EntityDefault.FieldsLength50),
                //ClientName = _clientInfoProvider.ComputerName.TruncateWithPostfix(EntityDefault.FieldsLength100),
                Id = Guid.NewGuid().ToString()
            };

            ActionExecutedContext result = null;
            try
            {
                result = await next();
                if (result.Exception != null && !result.ExceptionHandled)
                {
                    auditInfo.Exception = result.Exception;
                }
            }
            catch (Exception ex)
            {
                auditInfo.Exception = ex;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

                if (result != null)
                {
                    switch (result.Result)
                    {
                        case ObjectResult objectResult:
                            auditInfo.ReturnValue = JsonConvert.SerializeObject(objectResult.Value);
                            break;

                        case JsonResult jsonResult:
                            auditInfo.ReturnValue = JsonConvert.SerializeObject(jsonResult.Value);
                            break;

                        case ContentResult contentResult:
                            auditInfo.ReturnValue = contentResult.Content;
                            break;
                    }
                }
                Console.WriteLine(auditInfo.ToString());
                //保存审计日志
                await _auditLogService.SaveAsync(auditInfo);
            }
        }

        /// <summary>
        /// 是否需要记录审计
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool ShouldSaveAudit(ActionExecutingContext context)
        {
            if (!(context.ActionDescriptor is ControllerActionDescriptor))
                return false;
            var methodInfo = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo;

            if (methodInfo == null)
            {
                return false;
            }

            if (!methodInfo.IsPublic)
            {
                return false;
            }

            if (methodInfo.GetCustomAttribute<AuditedAttribute>() != null)
            {
                return true;
            }

            if (methodInfo.GetCustomAttribute<DisableAuditingAttribute>() != null)
            {
                return false;
            }

            var classType = methodInfo.DeclaringType;
            if (classType != null)
            {
                if (classType.GetTypeInfo().GetCustomAttribute<AuditedAttribute>() != null)
                {
                    return true;
                }

                if (classType.GetTypeInfo().GetCustomAttribute<AuditedAttribute>() != null)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
