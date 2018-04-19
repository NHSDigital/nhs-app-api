using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Models
{
    public class BadRequestErrorResponse
    {
        public string Message { get; set; }
        public Dictionary<string, string[]> ModelState { get; set; }
    }
}