using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nhs.App.Api.Integration.Tests
{
    public class TestConfiguration
    {
        private readonly TestContext _context;

        public string ApplicationUrl { get; }
        public string TokenEndpoint { get; }
        public string IssuerKey { get; }
        public string PrivateKeyFilePath { get; }
        public string KidValue { get; }
        public string SendToNhsNumber { get; }

        public string OdsCode { get; }

        public string RequesterDisplay { get; }
        public TestConfiguration(TestContext context)
        {
            _context = context;
            ApplicationUrl = GetTestPropertySetting("ApplicationUrl");
            SendToNhsNumber = GetTestPropertySetting("SendToNhsNumber");
            TokenEndpoint = GetTestPropertySetting("TokenEndpoint");
            IssuerKey = GetTestPropertySetting("IssuerKey");
            PrivateKeyFilePath = GetTestPropertySetting("PrivateKeyFilePath");
            KidValue = GetTestPropertySetting("KidValue");
            OdsCode = GetTestPropertySetting("OdsCode");
            RequesterDisplay = GetTestPropertySetting("RequesterDisplay");

            Console.WriteLine($"{nameof(TokenEndpoint)} - {TokenEndpoint}");
            Console.WriteLine($"{nameof(ApplicationUrl)} - {ApplicationUrl}");
            Console.WriteLine($"{nameof(IssuerKey)} - {IssuerKey.Length}");
            Console.WriteLine($"{nameof(PrivateKeyFilePath)} - {PrivateKeyFilePath.Length}");
            Console.WriteLine($"{nameof(KidValue)} - {KidValue.Length}");
            Console.WriteLine($"{nameof(OdsCode)} - {OdsCode}");
            Console.WriteLine($"{nameof(RequesterDisplay)} - {RequesterDisplay}");
        }

        private string GetTestPropertySetting(string propertyName)
        {
            return _context!.Properties[propertyName]?.ToString();
        }
    }
}
