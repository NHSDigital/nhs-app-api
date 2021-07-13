using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.Logs
{
    public abstract class DockerLogs
    {
        private readonly IEnumerable<string> _logs;
        private readonly Regex _searchRegex;
        private const int LogEndTimeExtensionMilliseconds = 1000;

        protected DockerLogs(DateTime startTime, DateTime endTime, string containerName, string regex)
        {
            _logs = UI.DockerLogs.GetDockerLogs(containerName, startTime, endTime, LogEndTimeExtensionMilliseconds);
            _searchRegex = new Regex(regex);
        }

        private bool HasMatches() => _logs.Any(x => _searchRegex.IsMatch(x));

        public void AssertFound() => Assert.IsTrue(HasMatches());
    }
}