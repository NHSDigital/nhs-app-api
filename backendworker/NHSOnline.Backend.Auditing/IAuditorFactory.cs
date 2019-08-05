using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Auditing
{
    public interface IAuditorFactory
    {
        IAuditor CreateAuditor(ILogger<Auditor> logger);
    }
}