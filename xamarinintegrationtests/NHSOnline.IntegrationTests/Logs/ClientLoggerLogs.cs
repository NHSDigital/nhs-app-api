using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.Logs
{
    internal class ClientLoggerLogs : DockerLogs
    {
        private static readonly Regex LogRegex = new Regex("CorrelationId=(?<correlation_id>([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})).*client_(?:information|error|debug)_message=\"(?<client_log>[^\"]*)\"");

        internal ClientLoggerLogs(DateTime startTime, DateTime endTime) : base(startTime, endTime, "log", LogRegex)
        {
        }

        internal void AssertClientLogPresent(string expectedClientLog)
        {
            foreach (var clientLog in ClientLogs())
            {
                if (expectedClientLog == clientLog)
                {
                    return;
                }
            }

            Assert.Fail(
                $"Expected at least one client log to be:\n{expectedClientLog}\n\nLogs were:\n\n{string.Join("\n\n", Logs)}");
        }

        private IEnumerable<string> ClientLogs()
        {
            foreach (var log in Logs)
            {
                var match = LogRegex.Match(log);
                var encodedClientLog = match.Groups["client_log"].Value;
                var clientLog = HttpUtility.UrlDecode(encodedClientLog);
                yield return clientLog;
            }
        }
    }
}