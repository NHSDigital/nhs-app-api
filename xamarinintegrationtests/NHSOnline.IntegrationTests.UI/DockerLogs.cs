using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NHSOnline.IntegrationTests.UI
{
    public sealed class DockerLogs
    {
        private readonly TestTempDirectory _testTempDirectory;
        private readonly DateTime _startTime = DateTime.UtcNow;

        internal DockerLogs(TestTempDirectory testTempDirectory)
        {
            _testTempDirectory = testTempDirectory;
        }

        internal void AttachDockerLogs(TestResultContext testResultContext)
        {
            using var listContainerNames = new ProcessRunner("docker", "ps -a --filter=network=int_test_default --format {{.Names}}").Start();

            foreach (var containerName in listContainerNames.Output())
            {
                testResultContext.TryCleanUp(
                    $"attach docker logs {containerName}",
                    () => AttachDockerLogs(testResultContext, containerName));
            }
        }

        public static IEnumerable<string> GetDockerLogs(string container, DateTime startTime, DateTime endTime)
        {
            using var listContainerNames =
                new ProcessRunner("docker", $"ps -a --filter=network=int_test_default --filter=name=int_test_{container} --format {{{{.Names}}}}").Start();

            var logLines = new List<string>();
            foreach (var containerName in listContainerNames.Output())
            {
                logLines.AddRange(GetLogLines(containerName, startTime, endTime));
            }

            return logLines;
        }

        private static IEnumerable<string> GetLogLines(string containerName, DateTime startTime, DateTime endTime)
        {
            var arguments = $"logs {containerName} --since {startTime:yyyy-MM-ddTHH:mm:ssZ} --until {endTime:yyyy-MM-ddTHH:mm:ssZ}";
            using var logs = new ProcessRunner("docker", arguments).Start();
            return logs.Output().ToArray();
        }

        private void AttachDockerLogs(TestResultContext testResultContext, string containerName)
        {
            var file = _testTempDirectory.GetTempFile($"{containerName}.log");

            var arguments = $"logs {containerName} --since {_startTime:yyyy-MM-ddTHH:mm:ssZ}";

            // Add timestamps to the web logs as they are not included in the log lines
            if (containerName.Contains("web", StringComparison.OrdinalIgnoreCase))
            {
                arguments += " --timestamps";
            }

            using var logs = new ProcessRunner("docker", arguments).Start();
            File.AppendAllLines(file.FullName, logs.Output());

            if (file.Exists)
            {
                testResultContext.Attach(file);
            }
        }
    }
}