using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models
{
    public class Header
    {
        public string Name { get; set; }
        public IEnumerable<Matcher> Matchers { get; set; }
    }
}
