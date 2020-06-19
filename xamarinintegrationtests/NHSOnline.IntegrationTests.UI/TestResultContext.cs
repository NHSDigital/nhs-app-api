using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.UI
{
    internal sealed class TestResultContext : IDriverCleanupContext
    {
        private readonly TestContext _testContext;
        private readonly TestResult _testResult;

        internal TestResultContext(TestContext testContext, TestResult testResult)
        {
            _testContext = testContext;
            _testResult = testResult;
        }

        public string TestName => _testContext.TestName;

        public void TryAttach(string name, Func<string> createFile)
        {
            TryCleanUp(
                $"attach {name}",
                () =>
                {
                    var filename = createFile();
                    _testContext.Logs.Info($"Attaching {name} from {filename} (Exists: {File.Exists(filename)})");
                    AddResultFile(filename);
                });
        }

        public void TryCleanUp(string description, Action cleanUp)
        {
            try
            {
                cleanUp();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _testContext.Logs.Error($"Failed to {description}: {e}");
            }
        }

        internal void AddResultFile(string filename)
        {
            if (_testResult.ResultFiles == null)
            {
                _testResult.ResultFiles = new List<string> { filename };
            }
            else
            {
                _testResult.ResultFiles.Add(filename);
            }
        }
    }
}