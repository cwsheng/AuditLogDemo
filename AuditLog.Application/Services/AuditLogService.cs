using AuditLog.EF;
using AuditLogDemo.EF;
using AuditLogDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditLogDemo.Services
{
    public class AuditLogService : IAuditLogService
    {
        IRepository<AuditInfo> _repository;
        public AuditLogService(IRepository<AuditInfo> repository)
        {
            _repository = repository;
        }

        public async Task SaveAsync(AuditInfo auditInfo)
        {
            _repository.Save(auditInfo);
        }

        public IEnumerable<AuditInfo> GetAll()
        {
            return _repository.GetAll();
        }

        public AuditInfo Get(int key)
        {
            return _repository.Get(key);
        }
    }
}
