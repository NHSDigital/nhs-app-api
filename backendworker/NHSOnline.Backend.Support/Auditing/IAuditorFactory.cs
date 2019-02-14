using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Auditing
{
    public interface IAuditorFactory
    {
        IAuditor CreateAuditor(ILogger<Auditor> logger);
    }
}