using System.Collections.Generic;
using System.Net;

namespace NHSOnline.Backend.Worker.Mocking.Models
{
    public class Response
    {
        public Response()
        {
            Status = (int) HttpStatusCode.OK;
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            };
        }

        public int Status { get; set; }
        public string Body { get; set; }
        public int? FixedDelayMilliseconds { get; set; }
        public IDictionary<string, string> Headers { get; set; }
    }
}
