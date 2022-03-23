using System;
using System.Collections.Concurrent;
using System.IO;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Logs
{
    internal class MocksLogs : IDisposable
    {
        private readonly ConcurrentQueue<string> _logs = new ConcurrentQueue<string>();

        public MocksLogs()
        {
            Init.Logged += MocksOnLogged;
        }

        public void Dispose()
        {
            UnsubscribeFromLogs();
        }

        private void UnsubscribeFromLogs()
        {
            Init.Logged -= MocksOnLogged;
        }

        private void MocksOnLogged(object _, string log)
        {
            _logs.Enqueue(log);
        }

        public void AttachMocksLogs(TestResultContext testResultContext)
        {
            UnsubscribeFromLogs();

            testResultContext.TryAttach("logs from http mocks", "httpmocks.log", (file) =>
            {
                File.WriteAllLines(file.FullName, _logs);
            });
        }
    }
}