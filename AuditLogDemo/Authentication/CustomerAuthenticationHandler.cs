using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AuditLogDemo.Authentication
{
    /// <summary>
    /// 自定义认证处理器
    /// </summary>
    public class CustomerAuthenticationHandler : IAuthenticationHandler
    {
        private IUserService _userService;
        public CustomerAuthenticationHandler(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 自定义认证Scheme名称
        /// </summary>
        public const string CustomerSchemeName = "cusAuth";

        private AuthenticationScheme _scheme;
        private HttpContext _context;

        /// <summary>
        /// 认证逻辑：认证校验主要逻辑
        /// </summary>
        /// <returns></returns>
        public Task<AuthenticateResult> AuthenticateAsync()
        {
            AuthenticateResult result;
            _context.Request.Headers.TryGetValue("Authorization", out StringValues values);
            string valStr = values.ToString();
            if (!string.IsNullOrWhiteSpace(valStr))
            {
                //认证模拟basic认证：cusAuth YWRtaW46YWRtaW4=
                string[] authVal = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(valStr.Substring(CustomerSchemeName.Length + 1))).Split(':');
                var loginInfo = new Dto.LoginDto() { Username = authVal[0], Password = authVal[1] };
                var validVale = _userService.IsValid(loginInfo);
                if (!validVale)
                    result = AuthenticateResult.Fail("未登陆");
                else
                {
                    var ticket = GetAuthTicket(loginInfo.Username, "admin");
                    result = AuthenticateResult.Success(ticket);
                }
            }
            else
            {
                result = AuthenticateResult.Fail("未登陆");
            }
            return Task.FromResult(result);
        }

        /// <summary>
        /// 未登录时的处理
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            _context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 权限不足时处理
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task ForbidAsync(AuthenticationProperties properties)
        {
            _context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 初始化认证
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            _scheme = scheme;
            _context = context;
            return Task.CompletedTask;
        }

        #region 认证校验逻辑

        private bool Valid()
        {
            _context.Request.Headers.TryGetValue("Authorization", out StringValues values);
            string valStr = values.ToString();
            if (!string.IsNullOrWhiteSpace(valStr))
            {
                //认证模拟basic认证：cusAuth YWRtaW46YWRtaW4=
                string[] authVal = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(valStr.Substring(CustomerSchemeName.Length + 1))).Split(':');
                var loginInfo = new Dto.LoginDto() { Username = authVal[0], Password = authVal[1] };
                return _userService.IsValid(loginInfo);
            }
            return false;
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
            }, CustomerSchemeName);
            var principal = new ClaimsPrincipal(claimsIdentity);
            return new AuthenticationTicket(principal, _scheme.Name);
        }

        #endregion
    }
}
