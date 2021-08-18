using System;
using System.Linq;
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
        public string ApigeeApiKey  { get; }
        public string[] SendToNhsNumbers { get; }
        public string SendToOdsCode { get; }

        public TestConfiguration(TestContext context)
        {
            _context = context;
            ApplicationUrl = GetTestPropertySetting("ApplicationUrl");
            ApigeeApiKey = GetTestPropertySetting("ApigeeApiKey");
            SendToNhsNumbers = GetTestPropertySetting("SendToNhsNumbers")
                .Split(',')
                .Select(x => x.Trim())
                .ToArray();
            SendToOdsCode = GetTestPropertySetting("SendToOdsCode");
            TokenEndpoint = GetTestPropertySetting("TokenEndpoint");
            IssuerKey = GetTestPropertySetting("IssuerKey");
            PrivateKeyFilePath = GetTestPropertySetting("PrivateKeyFilePath");
            KidValue = GetTestPropertySetting("KidValue");

            Console.WriteLine($"{nameof(TokenEndpoint)} - {TokenEndpoint}");
            Console.WriteLine($"{nameof(ApplicationUrl)} - {ApplicationUrl}");
            Console.WriteLine($"{nameof(IssuerKey)} - {IssuerKey.Length}");
            Console.WriteLine($"{nameof(PrivateKeyFilePath)} - {PrivateKeyFilePath.Length}");
            Console.WriteLine($"{nameof(KidValue)} - {KidValue.Length}");
        }

        private string GetTestPropertySetting(string propertyName)
        {
            return _context!.Properties[propertyName]?.ToString();
        }
    }
}
