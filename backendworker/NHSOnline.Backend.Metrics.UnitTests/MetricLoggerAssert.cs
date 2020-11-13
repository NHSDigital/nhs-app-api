using System;
using FluentAssertions;

namespace NHSOnline.Backend.Metrics.UnitTests
{
    internal static class MetricLoggerAssert
    {
        public static string AssertSingleLine(string consoleOut)
        {
            return consoleOut
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Should()
                .ContainSingle("a single metric log is expected")
                .Subject.Trim();
        }

        public static void AssertTimeStamp(string consoleOut)
        {
            AssertSingleLine(consoleOut)
                .Split(" ")[0].Should()
                .MatchRegex(@"^Timestamp=\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\d\:\d\d\d$");
        }

        public static void AssertContains(string consoleOut, string expected)
        {
            AssertSingleLine(consoleOut).Split(" ").Should().Contain(expected);
        }
    }
}