using System;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal interface IWebContext
    {
        internal T AssertPlatformWebContext<T>() where T: class, IWebContext
        {
            if (this is T platformWebContext)
            {
                return platformWebContext;
            }

            throw new InvalidOperationException(
                $"Expected IWebContext to be of type {typeof(T).Name} but was {GetType().Name}");
        }
    }
}