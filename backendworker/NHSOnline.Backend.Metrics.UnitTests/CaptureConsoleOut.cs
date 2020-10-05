using System;
using System.IO;

namespace NHSOnline.Backend.Metrics.UnitTests
{
    internal class CaptureConsoleOut : IDisposable
    {
        private readonly StringWriter _consoleOutput = new StringWriter();
        private readonly TextWriter _originalConsoleOutput;

        public CaptureConsoleOut()
        {
            _originalConsoleOutput = Console.Out;
            Console.SetOut(_consoleOutput);
        }

        public void Dispose()
        {
            Console.SetOut(_originalConsoleOutput);
            Console.Write(ToString());
            _consoleOutput.Dispose();
        }

        public override string ToString() => _consoleOutput.ToString();
    }
}