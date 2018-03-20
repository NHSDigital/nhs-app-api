using System.Collections.Generic;
using System.Net;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models
{
    public class Response
    {
        public const string DefaultBodyDestination = "SameAsSource";

        public Response()
        {
            StatusCode = (int) HttpStatusCode.OK;
            BodyDestination = DefaultBodyDestination;
            BodyEncoding = new BodyEncoding();
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            };
        }

        public int StatusCode { get; set; }
        public string BodyDestination { get; set; }
        public string Body { get; set; }
        public int? Delay { get; set; }
        public BodyEncoding BodyEncoding { get; set; }
        public bool UseTransformer { get; set; }
        public IDictionary<string, string> Headers { get; set; }
    }
}
