using System.IO;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public interface IAuditorFactory
    {
        IAuditor CreateAuditor(ILogger logger);
    }
}