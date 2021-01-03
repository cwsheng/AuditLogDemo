using AuditLogDemo.EF;
using AuditLogDemo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuditLog.EF
{
    public class AuditLogRepository : IRepository<AuditInfo>
    {
        AuditLogDBContent _auditLogDB;
        public AuditLogRepository(AuditLogDBContent auditLogDB)
        {
            _auditLogDB = auditLogDB;
        }

        public AuditInfo Save(AuditInfo entity)
        {
            var retEntity = _auditLogDB.AuditInfos.Add(entity);
            _auditLogDB.SaveChanges();
            return retEntity.Entity;
        }
    }
}
