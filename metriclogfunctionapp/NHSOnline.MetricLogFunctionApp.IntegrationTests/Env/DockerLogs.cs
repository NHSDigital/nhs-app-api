using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;

namespace NhsAppAnalytics.FunctionApp.IntegrationTests.Env
{
    internal sealed class DockerLogs
    {
        private readonly TestTempDirectory _testTempDirectory;
        private readonly DateTime _startTime = DateTime.UtcNow;

        internal DockerLogs(TestTempDirectory testTempDirectory)
        {
            _testTempDirectory = testTempDirectory;
        }

        internal void AttachDockerLogs(TestResultContext testResultContext)
        {
            using var listContainerNames = new ProcessRunner("docker", "ps -a --filter=network=analytics_default --format {{.Names}}").Start();

            foreach (var containerName in listContainerNames.Output())
            {
                testResultContext.TryCleanUp(
                    $"attach docker logs {containerName}",
                    () => AttachDockerLogs(testResultContext, containerName));
            }
        }

        private void AttachDockerLogs(TestResultContext testResultContext, string containerName)
        {
            var file = _testTempDirectory.GetTempFile($"{containerName}.log");

            var logs = FetchDockerLogs(containerName);
            File.AppendAllLines(file.FullName, logs);

            if (file.Exists)
            {
                testResultContext.Attach(file);
            }
        }

        internal List<string> FetchDockerLogs(string containerName)
        {
            var arguments = $"logs {containerName} --since {_startTime:yyyy-MM-ddTHH:mm:ssZ}";

            using var logs = new ProcessRunner("docker", arguments).Start();
            return logs.Output().ToList();
        }
    }
}