using System;
using System.Collections.Concurrent;
using System.IO;

namespace NHSOnline.IntegrationTests.UI
{
    internal class MocksLogs : IDisposable
    {
        private readonly ConcurrentQueue<string> _logs = new ConcurrentQueue<string>();

        public MocksLogs()
        {
            Mocks.Logged += MocksOnLogged;
        }

        public void Dispose()
        {
            UnsubscribeFromLogs();
        }

        private void UnsubscribeFromLogs()
        {
            Mocks.Logged -= MocksOnLogged;
        }

        private void MocksOnLogged(object? _, string log)
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