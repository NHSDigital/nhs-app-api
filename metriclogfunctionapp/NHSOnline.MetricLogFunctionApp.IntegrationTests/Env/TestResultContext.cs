using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env
{
    internal sealed class TestResultContext
    {
        private readonly TestEnv _testEnv;
        private readonly TestResult _testResult;

        internal TestResultContext(TestEnv testEnv, TestResult testResult)
        {
            _testEnv = testEnv;
            _testResult = testResult;
        }

        internal UnitTestOutcome Outcome => _testResult.Outcome;

        private TestLogs Logs => _testEnv.Logs;

        public void Attach(FileInfo file)
        {
            AddResultFile(file);
        }

        public void TryAttach(string name, string filename, Action<FileInfo> createFile)
        {
            TryCleanUp(
                $"attach {name} ({filename})",
                () =>
                {
                    var file = _testEnv.GetTempFile(filename);

                    createFile(file);

                    Logs.Info($"Attaching {name} from {file.FullName} (Exists: {file.Exists})");
                    AddResultFile(file);
                });
        }

        public async Task TryCleanUp(string description, Func<Task> cleanUp)
        {
            try
            {
                await cleanUp();
            }
            catch (Exception e)
            {
                Logs.Error($"Failed to {description}: {e}");
            }
        }

        public void TryCleanUp(string description, Action cleanUp)
        {
            try
            {
                cleanUp();
            }
            catch (Exception e)
            {
                Logs.Error($"Failed to {description}: {e}");
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