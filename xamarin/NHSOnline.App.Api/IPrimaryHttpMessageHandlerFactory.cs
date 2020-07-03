using System.Net.Http;

namespace NHSOnline.App.Api
{
    /// <summary>
    /// Platform specific implementation of this interface should be registered
    /// to return the native HTTP message handler for that platform.
    /// </summary>
    public interface IPrimaryHttpMessageHandlerFactory
    {
        HttpMessageHandler CreatePrimaryHttpMessageHandler();
    }
}
