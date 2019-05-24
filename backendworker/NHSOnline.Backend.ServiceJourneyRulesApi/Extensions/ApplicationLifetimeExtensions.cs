using System;
using Microsoft.Extensions.Hosting;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Extensions
{
    internal static class ApplicationLifetimeExtensions
    {
        public static void StopApplication(this IApplicationLifetime applicationLifetime, int exitCode)
        {
            Environment.ExitCode = exitCode;
            applicationLifetime.StopApplication();
        }  
    }
}