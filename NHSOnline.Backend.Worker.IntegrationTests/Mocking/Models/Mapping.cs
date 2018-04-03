namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models
{
    public class Mapping
    {
        public Mapping(Request request, Response response)
        {
            Request = request;
            Response = response;
        }

        public Mapping(Request request, Response response, int priority)
        {
            Request = request;
            Response = response;
            Priority = priority;
        }

        public int Priority { get; set; }
        public Request Request { get; set; }
        public Response Response { get; set; }
    }
}

