using AuditLogDemo.Dto;
using AuditLogDemo.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogDemo.Authentication
{
public class TokenAuthenticationService : IAuthenticateService
{
    private readonly IUserService _userService;
    private readonly JwtSetting _jwtSetting;

    public TokenAuthenticationService(IUserService userService, IOptions<JwtSetting> jwtSetting)
    {
        _userService = userService;
        _jwtSetting = jwtSetting.Value;
    }


    public bool IsAuthenticated(LoginDto request, out string token)
    {
        token = string.Empty;
        if (!_userService.IsValid(request))
            return false;
        var claims = new[] { new Claim(ClaimTypes.Name, request.Username) };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var jwtToken = new JwtSecurityToken(_jwtSetting.Issuer, _jwtSetting.Audience, claims, expires: DateTime.Now.AddMinutes(_jwtSetting.AccessExpiration), signingCredentials: credentials);
        token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        return true;
    }
}
}
