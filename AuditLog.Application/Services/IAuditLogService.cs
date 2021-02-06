using AuditLogDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditLogDemo.Services
{
    public interface IAuditLogService
    {
        Task SaveAsync(AuditInfo auditInfo);
        IEnumerable<AuditInfo> GetAll();
        AuditInfo Get(int key);
    }
}
