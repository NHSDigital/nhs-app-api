using System;
using System.Threading;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public sealed class WaitForAction
    {
        internal WaitForAction()
        {
        }

        public void WaitForBackAction()
        {
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }
    }
}