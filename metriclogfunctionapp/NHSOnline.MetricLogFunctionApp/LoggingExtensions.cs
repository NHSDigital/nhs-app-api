using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace NHSOnline.MetricLogFunctionApp
{
    public static class LoggingExtensions
    {
        public static void LogEnter(this ILogger logger, [CallerMemberName] string methodName = "", string reportName = null)
        {
            if (reportName != null)
            {
                logger.LogInformation("Entering {methodName} with Report '{reportName}'.", methodName, reportName);
            }
            else
            {
                logger.LogInformation("Entering {methodName}.", methodName);
            }
        }

        public static void LogExit(this ILogger logger, [CallerMemberName] string methodName = "", string reportName = null)
        {
            if (reportName != null)
            {
                logger.LogInformation("Exiting {methodName} with Report '{reportName}'.", methodName, reportName);
            }
            else
            {
                logger.LogInformation("Exiting {methodName}.", methodName);
            }
        }

        public static void LogMethodFailure(this ILogger logger, Exception e, [CallerMemberName] string methodName = "")
        {
            logger.LogError(e, "Failure {methodName}", methodName);
        }
    }
}
