using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.Logs
{
    public abstract class DockerLogs
    {
        private readonly IList<string> _logs;
        private readonly TimeSpan _logEndTimeExtension = new(0,0,1);

        protected DockerLogs(DateTime startTime, DateTime endTime, string containerName, string regex)
            : this(startTime, endTime, containerName, new Regex(regex))
        {
        }

        protected DockerLogs(DateTime startTime, DateTime endTime, string containerName, Regex regex)
        {
            _logs = UI.DockerLogs.GetDockerLogs(containerName, startTime, endTime.Add(_logEndTimeExtension))
                .Where(x => regex.IsMatch(x))
                .ToList();
        }

        private bool HasMatches() => _logs.Any();

        public void AssertFound() => Assert.IsTrue(HasMatches());

        protected IEnumerable<string> Logs => _logs;
    }
}