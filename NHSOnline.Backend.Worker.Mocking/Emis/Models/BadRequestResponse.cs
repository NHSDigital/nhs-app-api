using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Mocking.Emis.Models
{
    public class BadRequestResponse
    {
        public string Message { get; }

        public Dictionary<string, IEnumerable<string>> ModelState { get; }

        public BadRequestResponse(string message)
        {
            Message = message;
        }

        public BadRequestResponse(string message, string fieldName) :this(message)
        {
            ModelState = new Dictionary<string, IEnumerable<string>>
            {
                [$"request.{fieldName}"] = new[] { "An error has occurred." }
            };
        }
    }
}
