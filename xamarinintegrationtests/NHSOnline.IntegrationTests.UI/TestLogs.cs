using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI
{
    internal sealed class TestLogs
    {
        private readonly List<string> _info = new List<string>();
        private readonly List<string> _error = new List<string>();

        internal void Info(string format, params object[] args)
            => Info(string.Format(CultureInfo.InvariantCulture, format, args));
        internal void Info(string message) => _info.Add($"{DateTime.UtcNow:s} {message}");

        internal void Error(string format, params object[] args)
            => Error(string.Format(CultureInfo.InvariantCulture, format, args));
        internal void Error(string message) => _error.Add($"{DateTime.UtcNow:s} {message}");

        internal void UpdateResult(TestResult testResult)
        {
            testResult.LogOutput += string.Join(Environment.NewLine, _info);
            testResult.LogError += string.Join(Environment.NewLine, _error);
        }
    }
}