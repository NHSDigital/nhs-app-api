using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

        public void TryAttach(string name, string filename, Action<FileInfo> createFile)
        {
            TryCleanUp(
                $"attach {name} ({filename})",
                () =>
                {
                    var file = _testContext.GetTempFile(filename);

                    createFile(file);

                    _testContext.Logs.Info($"Attaching {name} from {file.FullName} (Exists: {file.Exists})");
                    AddResultFile(file);
                });
        }

        public void Attach(FileInfo file)
        {
            AddResultFile(file);
        }

        public void TryCleanUp(string description, Action cleanUp)
        {
            try
            {
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var cleanUpTask = Task.Run(cleanUp, cancellationTokenSource.Token);
                cleanUpTask.Wait(cancellationTokenSource.Token);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _testContext.Logs.Error($"Failed to {description}: {e}");
            }
        }

        private void AddResultFile(FileInfo file)
        {
            if (_testResult.ResultFiles == null)
            {
                _testResult.ResultFiles = new List<string> { file.FullName };
            }
            else
            {
                _testResult.ResultFiles.Add(file.FullName);
            }
        }
    }
}