using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Login;

namespace NHSOnline.MetricLogFunctionApp.Database.EntityFramework
{
    public class TransactionContext : DbContext
    {
        private readonly IEnumerable<IDbConnectionInterceptor> _interceptors;

        public TransactionContext(
            DbContextOptions<TransactionContext> options,
            IEnumerable<IDbConnectionInterceptor> interceptors)
            : base(options)
        {
            _interceptors = interceptors;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.AddInterceptors(_interceptors);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConsentMetric>().HasKey(o => new { o.LoginId, o.Timestamp, o.OdsCode });
            modelBuilder.Entity<LoginMetric>().HasKey(o => new { o.Timestamp, o.LoginId, o.LoginEventId });
        }
    }
}