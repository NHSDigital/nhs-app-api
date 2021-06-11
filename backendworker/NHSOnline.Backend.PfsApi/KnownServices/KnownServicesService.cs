using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSOnline.Backend.PfsApi.Areas.KnownServices;
using NHSOnline.Backend.PfsApi.Areas.KnownServices.Models;

namespace NHSOnline.Backend.PfsApi.KnownServices
{
    internal sealed class KnownServicesService: IKnownServicesService
    {
        private readonly GetKnownServicesV3Result _v3Result;

        [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "ValidateAndLog covers this")]
        public KnownServicesService(
           KnownServicesV3 knownServicesV3)
        {
            knownServicesV3.Validate();

            _v3Result = new GetKnownServicesV3Result(
                knownServicesV3.ServicesV3.Values.ToList());
        }

        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Intentional; do not wish consumers to treat this as a property")]
        public GetKnownServicesV3Result GetConfiguration()
        {
            return _v3Result;
        }
    }
}