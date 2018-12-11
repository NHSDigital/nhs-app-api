using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Extensions
{
    public static class GetConfigurationExtensions
    {
        public static IEnumerable<PatientNhsNumber> ExtractNhsNumbers(this VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse> response)
        {
            var nhsNumbers = response.Body.Configuration.Account.PatientNumbers
                .Select(x => new PatientNhsNumber
                {
                    NhsNumber = x.Number.FormatToNhsNumber()
                });

            return nhsNumbers;
        }
    }
}