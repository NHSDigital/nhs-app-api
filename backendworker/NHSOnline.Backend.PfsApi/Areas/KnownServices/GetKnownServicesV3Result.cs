using System.Collections.Generic;
using NHSOnline.Backend.PfsApi.Areas.KnownServices.Models;

namespace NHSOnline.Backend.PfsApi.Areas.KnownServices
{
    public sealed class GetKnownServicesV3Result
    {
        public GetKnownServicesV3Result(
            List<KnownServiceV3> knownService)
        {
            V3Response = new GetKnownServicesV3Response
            {
                KnownServices = knownService
            };
        }

        public GetKnownServicesV3Response V3Response { get; }
    }
}