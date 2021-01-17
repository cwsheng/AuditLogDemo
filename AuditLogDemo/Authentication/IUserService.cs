using AuditLogDemo.Dto;

namespace AuditLogDemo.Authentication
{
public interface IUserService
{
    bool IsValid(LoginDto request);
}
}