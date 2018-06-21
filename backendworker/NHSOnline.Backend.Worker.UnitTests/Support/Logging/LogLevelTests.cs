using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Support.Logging;
using System;
using System.IO;

namespace NHSOnline.Backend.Worker.UnitTests.Support.Logging
{
    [TestClass]
    public class LogLevelTests
    {
        private const string LogFormat = "This is a log at level '{0}'";

        private const string LoggedMessageFormat = /* | Timestamp | Scope */ "| LogLevelTests | {0} | " + LogFormat + " |";

        private Stream _stream;

        [TestInitialize]
        public void TestInitialise()
        {
            _stream = new MemoryStream();
        }

        [TestCleanup]
        public void TestTeardown()
        {
            _stream.Close();
            _stream.Dispose();
        }

        private StreamReader Log(LogLevel logLevel)
        {
            var logProvider = new HttpContexedLoggerProvider(new StreamWriter(_stream), logLevel);
            return Log(logProvider);
        }

        private StreamReader Log(HttpContexedLoggerProvider logProvider)
        {
            var logger = logProvider.CreateLogger("LogLevelTests");
            logger.LogTrace(LogFormat, LogLevel.Trace);
            logger.LogDebug(LogFormat, LogLevel.Debug);
            logger.LogInformation(LogFormat, LogLevel.Information);
            logger.LogWarning(LogFormat, LogLevel.Warning);
            logger.LogError(LogFormat, LogLevel.Error);
            logger.LogCritical(LogFormat, LogLevel.Critical);
            _stream.Position = 0;
            return new StreamReader(_stream);
        }


        [TestMethod]
        public void LogRange()
        {
            // Generate the log messages for a level range.
            var streamReader = Log(new HttpContexedLoggerProvider(new StreamWriter(_stream), LogLevel.Information, "Debug", LogLevel.Critical, "Error"));

            // Check messages...
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Debug));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Information));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Warning));
            // End of generated log messages.
            streamReader.ReadLine().Should().BeNull();
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void InvalidMinLogLevel()
        {
            // Try and set an invalid log level, to throw exception
            var provider = new HttpContexedLoggerProvider(new StreamWriter(_stream), LogLevel.Debug, "dsfdsa", LogLevel.Error, null);
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void InvalidMaxLogLevelLimit()
        {
            // Try and set an invalid log level, to throw exception
            var provider = new HttpContexedLoggerProvider(new StreamWriter(_stream), LogLevel.Debug, null, LogLevel.Error, "dgdfs");
        }

        [TestMethod]
        public void LogSetAtTrace()
        {
            // Generate the log messages for this level...
            var streamReader = Log(LogLevel.Trace);

            // Check messages...
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Trace));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Debug));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Information));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Warning));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Error));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Critical));
            // End of generated log messages.
            streamReader.ReadLine().Should().BeNull();
        }

        [TestMethod]
        public void LogSetAtDebug()
        {
            // Generate the log messages for this level...
            var streamReader = Log(LogLevel.Debug);

            // Check messages...
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Debug));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Information));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Warning));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Error));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Critical));
            // End of generated log messages.
            streamReader.ReadLine().Should().BeNull();
        }

        [TestMethod]
        public void LogSetAtInformation()
        {
            // Generate the log messages for this level...
            var streamReader = Log(LogLevel.Information);

            // Check messages...
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Information));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Warning));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Error));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Critical));
            // End of generated log messages.
            streamReader.ReadLine().Should().BeNull();
        }

        [TestMethod]
        public void LogSetAtWarning()
        {
            // Generate the log messages for this level...
            var streamReader = Log(LogLevel.Warning);

            // Check messages...
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Warning));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Error));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Critical));
            // End of generated log messages.
            streamReader.ReadLine().Should().BeNull();
        }

        [TestMethod]
        public void LogSetAtError()
        {
            // Generate the log messages for this level...
            var streamReader = Log(LogLevel.Error);

            // Check messages...
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Error));
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Critical));
            // End of generated log messages.
            streamReader.ReadLine().Should().BeNull();
        }

        [TestMethod]
        public void LogSetAtCritical()
        {
            // Generate the log messages for this level...
            var streamReader = Log(LogLevel.Critical);

            // Check messages...
            streamReader.ReadLine().Should().Contain(string.Format(LoggedMessageFormat, LogLevel.Critical));
            // End of generated log messages.
            streamReader.ReadLine().Should().BeNull();
        }
        
        [TestMethod]
        public void LogSetAtNone()
        {
            // Generate the log messages for this level...
            var streamReader = Log(LogLevel.None);
            
            // Check no logs produced
            streamReader.ReadLine().Should().BeNull();
        }
    }
}
