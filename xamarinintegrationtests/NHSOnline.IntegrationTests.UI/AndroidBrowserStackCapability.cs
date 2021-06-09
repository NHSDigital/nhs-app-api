using System;

namespace NHSOnline.IntegrationTests.UI
{
    [Flags]
    public enum AndroidBrowserStackCapability
    {
        None = 0,
        SignInToGoogle = 1
    }
}