using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NHSOnline.IntegrationTests.UI
{
    internal sealed class ProcessRunner
    {
        private readonly List<Action<ProcessStartInfo>> _startInfoSetup = new List<Action<ProcessStartInfo>>();

        internal ProcessRunner(string filename, string arguments)
        {
            CustomiseStartInfo(
                si =>
                {
                    si.FileName = filename;
                    si.Arguments = arguments;
                    si.UseShellExecute = false;
                    si.CreateNoWindow = true;
                    si.RedirectStandardError = true;
                    si.RedirectStandardOutput = true;
                    si.RedirectStandardInput = true;
                });
        }

        internal ProcessRunner CustomiseStartInfo(Action<ProcessStartInfo> customiseStartInfo)
        {
            _startInfoSetup.Add(customiseStartInfo);
            return this;
        }

        internal RunningProcess Start()
        {
            var process = new Process();
            _startInfoSetup.ForEach(customise => customise(process.StartInfo));

            return new RunningProcess(process);
        }
    }
}