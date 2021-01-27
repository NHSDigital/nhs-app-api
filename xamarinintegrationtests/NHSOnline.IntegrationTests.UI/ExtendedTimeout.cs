using System;
using System.Threading;

namespace NHSOnline.IntegrationTests.UI
{
    public static class ExtendedTimeout
    {
        private static readonly ThreadLocal<TimeSpan> Timeout = new ThreadLocal<TimeSpan>(() => TimeSpan.FromSeconds(2), false);

        public static IDisposable FromSeconds(int seconds)
        {
            var revert = new ResetTimeout(Value);
            Timeout.Value = TimeSpan.FromSeconds(seconds);
            return revert;
        }

        internal static TimeSpan Value => Timeout.Value;

        private sealed class ResetTimeout : IDisposable
        {
            private readonly TimeSpan _previousValue;

            public ResetTimeout(TimeSpan value) => _previousValue = value;

            public void Dispose() => Timeout.Value = _previousValue;
        }
    }
}
