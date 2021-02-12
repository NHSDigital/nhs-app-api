using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NHSOnline.IntegrationTests.UI.Reporting;

namespace NHSOnline.IntegrationTests.UI
{
    internal sealed class TestLogs
    {
        private readonly List<string> _info = new();
        private readonly List<string> _error = new();

        private readonly TestReport _testReport;

        public TestLogs(TestReport testReport)
        {
            _testReport = testReport;
        }

        internal void Info(string format, params object[] args)
            => Info(string.Format(CultureInfo.InvariantCulture, format, args));
        internal void Info(string message) => Log(_info, message);

        internal void Error(string format, params object[] args)
            => Error(string.Format(CultureInfo.InvariantCulture, format, args));
        internal void Error(string message) => Log(_error, message);

        private static void Log(List<string> logs, string message)
        {
            var logLine = $"{DateTime.UtcNow:s} {message}";
            System.Diagnostics.Debug.WriteLine(logLine);
            logs.Add(logLine);
        }

        internal void UpdateResult(TestResult testResult)
        {
            Info("TestReport:{0}", JsonConvert.SerializeObject(_testReport));
            testResult.LogOutput += string.Join(Environment.NewLine, _info);
            testResult.LogError += string.Join(Environment.NewLine, _error);
        }
    }
}