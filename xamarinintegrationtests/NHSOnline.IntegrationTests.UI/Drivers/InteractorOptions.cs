using System;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public sealed class InteractorOptions
    {
        internal TimeSpan Timeout { get; private set; } = TimeSpan.FromSeconds(2);

        public InteractorOptions SetTimeout(TimeSpan timeout)
        {
            Timeout = timeout;
            return this;
        }
    }
}