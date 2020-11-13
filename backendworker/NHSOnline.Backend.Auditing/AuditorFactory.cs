using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Auditing
{
    public class AuditorFactory
    {
        private readonly IConfiguration _configuration;
        private readonly AsyncLocal<HttpContextAuditorScope> _scopeProvider;
        private readonly IAuditSink _auditSink;

        public AuditorFactory(IConfiguration configuration, IAuditSink auditSink)
        {
            _configuration = configuration;
            _scopeProvider = new AsyncLocal<HttpContextAuditorScope>();
            _auditSink = auditSink;
        }

        public IAuditor CreateAuditor(ILogger logger)
        {
            return new Auditor(_scopeProvider, logger, _configuration, _auditSink);
        }

        public static IAuditor BuildAuditor(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetService<ILogger<Auditor>>();
            return serviceProvider.GetService<AuditorFactory>().CreateAuditor(logger);
        }
    }
}
