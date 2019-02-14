using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class BadRequestErrorResponse
    {
        public string Message { get; set; }
        public Dictionary<string, string[]> ModelState { get; set; }
    }
}