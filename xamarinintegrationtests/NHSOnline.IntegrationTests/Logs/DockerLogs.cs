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

        protected DockerLogs(DateTime startTime, DateTime endTime, string containerName, string regex)
        {
            _logs = UI.DockerLogs.GetDockerLogs(containerName, startTime, endTime);
            _searchRegex = new Regex(regex);
        }

        private bool HasMatches() => _logs.Select(x => _searchRegex.Match(x)).Any();

        public void AssertFound() => Assert.IsTrue(HasMatches());
    }
}