using System.Net;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.IOS
{
    internal sealed class IOSConfig
    {
        public string App { get; set; } = $"{Dns.GetHostName()}-ios";
    }
}