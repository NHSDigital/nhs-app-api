using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Extensions
{
    public static class AuthenticateReplyExtensions
    {
        public static IEnumerable<PatientNhsNumber> ExtractNhsNumbers(this AuthenticateReply authenticateReply)
        {
            var nhsNumber = authenticateReply?.User?.Person?.NationalId?.Value;
            var nhsNumbers = Enumerable.Empty<PatientNhsNumber>();

            if (nhsNumber != null)
            {
                nhsNumbers = new List<PatientNhsNumber>
                {
                    new PatientNhsNumber
                    {
                        NhsNumber = nhsNumber.FormatToNhsNumber()
                    }
                };
            }

            return nhsNumbers;
        }
    }
}
