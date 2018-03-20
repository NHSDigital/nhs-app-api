using System;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models
{
    public class Mapping
    {
        public Mapping()
        {
            Guid = Guid.NewGuid();
        }

        public Mapping(Request request, Response response): this()
        {
            Request = request;
            Response = response;
        }

        public Guid Guid { get; set; }
        public int Priority { get; set; }
        public Request Request { get; set; }
        public Response Response { get; set; }
    }
}

