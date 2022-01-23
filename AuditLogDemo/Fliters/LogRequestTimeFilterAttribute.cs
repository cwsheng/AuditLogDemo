using AuditLogDemo.EventSources;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AuditLogDemo.Fliters
{
public class ApiRequestTimeFilterAttribute : ActionFilterAttribute
{

    readonly Stopwatch _stopwatch = new Stopwatch();
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        _stopwatch.Start();
    }

    public override void OnResultExecuted(ResultExecutedContext context)
    {
        _stopwatch.Stop();
        ApiEventCounterSource.Log.Request(context.HttpContext.Request.GetDisplayUrl(), _stopwatch.ElapsedMilliseconds);
    }
}
}
