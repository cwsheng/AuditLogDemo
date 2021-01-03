using System;
using System.Collections.Generic;
using System.Text;

namespace AuditLog.EF
{
    public interface IRepository<T>
    {
        T Save(T entity);
    }
}
