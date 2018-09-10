using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public class AuditorFactory : IAuditorFactory
    {
        private readonly IAuditSink _auditSink;
        private readonly AsyncLocal<HttpContextAuditorScope> _scopeProvider;

        public AuditorFactory(IAuditSink auditSink)
        {
            _auditSink = auditSink;
            _scopeProvider = new AsyncLocal<HttpContextAuditorScope>();
        }

        public IAuditor CreateAuditor(ILogger<Auditor> logger)
        {
            return new Auditor(_auditSink, _scopeProvider, logger);
        }

        public static IAuditor BuildAuditor(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetService<ILogger<Auditor>>();
            return serviceProvider.GetService<IAuditorFactory>().CreateAuditor(logger);
        }
    }
}