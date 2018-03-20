using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models
{
    public class Request
    {
        public string Path { get; set; }
        public IList<string> Methods { get; set; }
        public IList<Header> Headers { get; set; }
        public IList<Param> Params { get; set; }
        public Body Body { get; set; }
    }
}
