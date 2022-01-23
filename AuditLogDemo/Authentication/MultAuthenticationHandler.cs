using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AuditLogDemo.Authentication
{

/// <summary>
/// 方式二：同时支持多种认证方式
/// </summary>
public class MultAuthenticationHandler : JwtBearerHandler
{
    public const string MultAuthName = "MultAuth";
    IUserService _userService;
    public MultAuthenticationHandler(IOptionsMonitor<JwtBearerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUserService userService)
        : base(options, logger, encoder, clock)
    {
        _userService = userService;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Context.Request.Headers.TryGetValue("Authorization", out StringValues values);
        string valStr = values.ToString();
        if (valStr.StartsWith(CustomerAuthenticationHandler.CustomerSchemeName))
        {
            var result = Valid();
            if (result != null)
                return Task.FromResult(AuthenticateResult.Success(result));
            else
                return Task.FromResult(AuthenticateResult.Fail("未认证"));
        }
        else
            return base.AuthenticateAsync();
    }

    private AuthenticationTicket Valid()
    {
        Context.Request.Headers.TryGetValue("Authorization", out StringValues values);
        string valStr = values.ToString();
        if (!string.IsNullOrWhiteSpace(valStr))
        {
            //认证模拟basic认证：cusAuth YWRtaW46YWRtaW4=
            string[] authVal = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(valStr.Substring(CustomerAuthenticationHandler.CustomerSchemeName.Length + 1))).Split(':');
            var loginInfo = new Dto.LoginDto() { Username = authVal[0], Password = authVal[1] };
            if (_userService.IsValid(loginInfo))
                return GetAuthTicket(loginInfo.Username, "admin");
        }
        return null;
    }

    /// <summary>
    /// 生成认证票据
    /// </summary>
    /// <param name="name"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    private AuthenticationTicket GetAuthTicket(string name, string role)
    {
        var claimsIdentity = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Role, role),
        }, CustomerAuthenticationHandler.CustomerSchemeName);
        var principal = new ClaimsPrincipal(claimsIdentity);
        return new AuthenticationTicket(principal, CustomerAuthenticationHandler.CustomerSchemeName);
    }
}
}
