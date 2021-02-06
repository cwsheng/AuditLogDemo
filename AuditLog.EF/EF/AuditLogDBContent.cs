using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace AuditLogDemo.EF
{
    public class AuditLogDBContent : DbContext
    {
        public AuditLogDBContent([NotNull] DbContextOptions options) : base(options)
        {
        }

        protected AuditLogDBContent()
        {
        }

        public virtual DbSet<Models.AuditInfo> AuditInfos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.AuditInfo>().HasKey(k => k.Id);
        }
    }
}
