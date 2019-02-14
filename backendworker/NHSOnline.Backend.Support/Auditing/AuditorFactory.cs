using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Auditing
{
    public class AuditorFactory : IAuditorFactory
    {
        private readonly IAuditSink _auditSink;
        private readonly IConfiguration _configuration;
        private readonly AsyncLocal<HttpContextAuditorScope> _scopeProvider;

        public AuditorFactory(IAuditSink auditSink, IConfiguration configuration)
        {
            _auditSink = auditSink;
            _configuration = configuration;
            _scopeProvider = new AsyncLocal<HttpContextAuditorScope>();
        }

        public IAuditor CreateAuditor(ILogger<Auditor> logger)
        {
            return new Auditor(_auditSink, _scopeProvider, logger, _configuration);
        }

        public static IAuditor BuildAuditor(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetService<ILogger<Auditor>>();
            return serviceProvider.GetService<IAuditorFactory>().CreateAuditor(logger);
        }
    }
}
