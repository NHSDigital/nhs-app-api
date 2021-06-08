using System;
using NHSOnline.IntegrationTests.UI.DeviceProperties;

namespace NHSOnline.IntegrationTests.Logs
{
    public class UserAgentDockerLogs : DockerLogs
    {
        private static string BuildRegexString(Platform platform) =>
            $"UserAgent={platform.UserAgentDeviceTypePrefix()}.+\\d.\\d.+nhsapp-manufacturer.+nhsapp-model.+nhsapp-os.+nhsapp-architecture.+";

        private UserAgentDockerLogs(DateTime startTime, DateTime endTime, string containerName, Platform platform) :
            base(startTime, endTime, containerName, BuildRegexString(platform))
        {
        }

        public static UserAgentDockerLogs GetLogs(DateTime startTime, DateTime endTime, string containerName,
            Platform platform) => new(startTime, endTime, containerName, platform);
    }
}