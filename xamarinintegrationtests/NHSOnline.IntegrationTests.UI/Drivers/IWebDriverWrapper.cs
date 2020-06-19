using System;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IWebDriverWrapper : IWebInteractor, IDriverWrapper
    {
        void GoToUrl(Uri uri);
    }
}