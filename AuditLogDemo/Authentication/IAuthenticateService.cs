using AuditLogDemo.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuditLogDemo.Authentication
{
public interface IAuthenticateService
{
    bool IsAuthenticated(LoginDto request, out string token);
}
}
