using System;

namespace NHSOnline.IntegrationTests.Logs
{
    public class SessionCorrelationIdDockerLogs : DockerLogs
    {
        private static string BuildRegexString() =>
            @"RequestPath:\/v1\/session.*Correlation ID ({{0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}) was found in the request headers";

        private SessionCorrelationIdDockerLogs(DateTime startTime, DateTime endTime, string containerName) :
            base(startTime, endTime, containerName, BuildRegexString())
        {
        }

        public static SessionCorrelationIdDockerLogs GetLogs(DateTime startTime, DateTime endTime, string containerName)
            => new(startTime, endTime, containerName);
    }
}