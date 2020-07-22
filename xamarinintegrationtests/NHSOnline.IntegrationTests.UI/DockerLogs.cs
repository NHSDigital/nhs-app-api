using System;
using System.IO;

namespace NHSOnline.IntegrationTests.UI
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
            using var listContainerNames = new ProcessRunner("docker", "ps -a --filter=network=int_test_default --format {{.Names}}").Start();

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