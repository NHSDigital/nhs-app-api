using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Suppliers.Emis.Models
{
    public class CreateSessionResponseModel
    {
        public string SessionId { get; set; }
        public IEnumerable<UserPatientLinkModel> UserPatientLinks { get; set; }
    }
}