using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NhsAppAnalytics.FunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Storage;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Logs;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env
{
    public sealed class TestEnv: IDisposable
    {
        private readonly TestTempDirectory _testTempDirectory;
        private readonly DockerLogs _dockerLogs;
        private readonly MocksLogs _mocksLogs;

        internal TestEnv(TestLogs logs)
        {
            Logs = logs;

            _testTempDirectory = new TestTempDirectory();
            _dockerLogs = new DockerLogs(_testTempDirectory);
            _mocksLogs = new MocksLogs();

            Queues = new StorageQueuesWrapper(logs);
            HttpEndpointCallers = new HttpEndpointCallers(logs);
            Postgres = new PostgresWrapper(logs);
        }

        internal TestLogs Logs { get; }

        internal StorageQueuesWrapper Queues { get; }
        internal HttpEndpointCallers HttpEndpointCallers { get; }
        internal PostgresWrapper Postgres { get; }

        internal List<string> FunctionLogs() => _dockerLogs.FetchDockerLogs("metriclog_function_1");

        internal async Task Initialise()
        {
            await Queues.Initialise();
            await Postgres.Initialise();
        }

        internal async Task CleanUp(TestResultContext testResultContext)
        {
            await Postgres.CleanUp(testResultContext);

            if (testResultContext.Outcome != UnitTestOutcome.Passed)
            {
                _dockerLogs.AttachDockerLogs(testResultContext);
                _mocksLogs.AttachMocksLogs(testResultContext);
            }
        }

        internal FileInfo GetTempFile(string filename) => _testTempDirectory.GetTempFile(filename);

        public void Dispose()
        {
            _mocksLogs?.Dispose();
            HttpEndpointCallers?.Dispose();
        }
    }
}