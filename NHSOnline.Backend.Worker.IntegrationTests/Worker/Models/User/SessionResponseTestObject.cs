using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.IntegrationTests.Worker.Models.Patient;

namespace NHSOnline.Backend.Worker.IntegrationTests.Worker.Models.User
{
    public class SessionResponseTestObject
    {
        public UserSessionResponse UserSessionResponse { get; set; }

        public string Cookie { get; set; }

    }
}
