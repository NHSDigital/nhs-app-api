using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace NHSOnline.IntegrationTests.UI
{
    internal sealed class RunningProcess: IDisposable
    {
        private readonly Process _process;

        private readonly ConcurrentQueue<string> _output = new ConcurrentQueue<string>();
        private readonly ManualResetEventSlim _outputAvailable = new ManualResetEventSlim(false);

        private readonly ManualResetEventSlim _outputDataComplete = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _errorDataComplete = new ManualResetEventSlim(false);

        internal RunningProcess(Process process)
        {
            _process = process ?? throw new ArgumentNullException(nameof(process));

            _process.OutputDataReceived += ProcessOnOutputDataReceived;
            _process.ErrorDataReceived += ProcessOnErrorDataReceived;

            _process.Start();

            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        internal IEnumerable<string> Output()
        {
            while (Running)
            {
                _outputAvailable.Reset();
                while (_output.TryDequeue(out var message))
                {
                    yield return message;
                }

                WaitHandle.WaitAny(new[] {_outputDataComplete.WaitHandle, _errorDataComplete.WaitHandle, _outputAvailable.WaitHandle});
            }

            while (_output.TryDequeue(out var message))
            {
                yield return message;
            }
        }

        private bool Running => !(_outputDataComplete.IsSet && _errorDataComplete.IsSet);

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            if (dataReceivedEventArgs.Data == null)
            {
                _errorDataComplete.Set();
            }
            else if (!string.IsNullOrWhiteSpace(dataReceivedEventArgs.Data))
            {
                _output.Enqueue(dataReceivedEventArgs.Data);
                _outputAvailable.Set();
            }
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            if (dataReceivedEventArgs.Data == null)
            {
                _outputDataComplete.Set();
            }
            else if (!string.IsNullOrWhiteSpace(dataReceivedEventArgs.Data))
            {
                _output.Enqueue(dataReceivedEventArgs.Data);
                _outputAvailable.Set();
            }
        }

        public void Dispose()
        {
            _process.OutputDataReceived -= ProcessOnOutputDataReceived;
            _process.ErrorDataReceived -= ProcessOnErrorDataReceived;
            _process.Dispose();

            _outputDataComplete.Dispose();
            _errorDataComplete.Dispose();
            _outputAvailable.Dispose();
        }
    }
}