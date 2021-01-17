using AuditLogDemo.Dto;

namespace AuditLogDemo.Authentication
{
    public class UserService : IUserService
    {
        public bool IsValid(LoginDto request)
        {
            return request.Password == request.Username && request.Username == "admin";
        }
    }
}