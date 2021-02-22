using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NHSOnline.IntegrationTests.UI.Reporting;
using OpenQA.Selenium.Remote;

namespace NHSOnline.IntegrationTests.UI
{
    internal sealed class TestLogs
    {
        private static readonly JsonSerializerSettings TestReportSerializerSettings = CreateTestReportSerializerSettings();

        private readonly List<string> _info = new();
        private readonly List<string> _error = new();

        private readonly TestReport _testReport;

        public TestLogs(TestReport testReport)
        {
            _testReport = testReport;
        }

        internal void BrowserStackSessionId(SessionId sessionId) => _testReport.BrowserStackSessionId = sessionId.ToString();

        internal void Info(string format, params object[] args)
            => Info(string.Format(CultureInfo.InvariantCulture, format, args));
        internal void Info(string message) => Log(_info, message);

        internal void Error(string format, params object[] args)
            => Error(string.Format(CultureInfo.InvariantCulture, format, args));
        internal void Error(string message) => Log(_error, message);

        internal void TestDevice(string name, string operatingSystemVersion)
            => _testReport.TestDevice(name, operatingSystemVersion);

        internal void UpdateResult(TestResult testResult, RetryStatus retryStatus)
        {
            _testReport.SetResult(testResult, retryStatus);

            var testReportJson = JsonConvert.SerializeObject(_testReport, TestReportSerializerSettings);
            Info("TestReport:{0}", testReportJson);

            testResult.LogOutput += string.Join(Environment.NewLine, _info);
            testResult.LogError += string.Join(Environment.NewLine, _error);
        }

        private static void Log(List<string> logs, string message)
        {
            var logLine = $"{DateTime.UtcNow:s} {message}";
            System.Diagnostics.Debug.WriteLine(logLine);
            logs.Add(logLine);
        }

        private static JsonSerializerSettings CreateTestReportSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            settings.Converters.Add(new StringEnumConverter());
            return settings;
        }
    }
}