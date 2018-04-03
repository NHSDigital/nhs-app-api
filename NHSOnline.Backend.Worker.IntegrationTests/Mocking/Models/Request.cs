using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models
{
    public class Request
    {

        public string UrlPath { get; set; }
        public string UrlPathPattern { get; set; }
        public string Method { get; set; }
        public readonly Dictionary<string, Dictionary<string, string>> Headers = new Dictionary<string, Dictionary<string, string>>();
        public readonly Dictionary<string, Dictionary<string, string>> QueryParameters = new Dictionary<string, Dictionary<string, string>>();
        public readonly IList<Dictionary<string, string>> BodyPatterns = new List<Dictionary<string, string>>();
    }
}
