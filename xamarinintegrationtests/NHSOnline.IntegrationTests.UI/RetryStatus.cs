namespace NHSOnline.IntegrationTests.UI
{
    internal abstract class RetryStatus
    {
        internal static RetryStatus NoRetry { get; } = new NoRetryStatus();

        internal static RetryStatus Retry(string category) => new DoRetryStatus(category);

        internal abstract bool ShouldRetry { get; }
        internal abstract string? Category { get; }

        private sealed class DoRetryStatus : RetryStatus
        {
            public DoRetryStatus(string category) => Category = category;

            internal override bool ShouldRetry => true;
            internal override string? Category { get; }
            public override string ToString() => "Should Retry";
        }

        private class NoRetryStatus : RetryStatus
        {
            internal override bool ShouldRetry => false;
            internal override string? Category => null;
            public override string ToString() => "Shouldn't Retry";
        }
    }
}