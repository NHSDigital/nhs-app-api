using System;

namespace NHSOnline.IntegrationTests.Logs
{
    public class UserAgentDockerLogs : DockerLogs
    {
        private static string BuildRegexString(Platform platform) =>
            $"UserAgent=nhsapp-{platform.ToString()}.+\\d.\\d.+nhsapp-manufacturer.+nhsapp-model.+nhsapp-os.+nhsapp-architecture.+";

        public enum Platform
        {
            ios,
            android
        }

        private UserAgentDockerLogs(DateTime startTime, DateTime endTime, string containerName, Platform platform) :
            base(startTime, endTime, containerName, BuildRegexString(platform))
        {
        }

        public static UserAgentDockerLogs GetLogs(DateTime startTime, DateTime endTime, string containerName,
            Platform platform) => new(startTime, endTime, containerName, platform);
    }
}