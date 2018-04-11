using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Mocking.Models
{
    public class Param
    {
        public string Name { get; set; }
        public IList<string> Values { get; set; }
    }
}
