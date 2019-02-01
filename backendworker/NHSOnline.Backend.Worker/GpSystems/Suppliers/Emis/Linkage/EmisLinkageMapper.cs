using System;
using NHSOnline.Backend.Worker.GpSystems.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Verifications;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage
{
    public class EmisLinkageMapper : IEmisLinkageMapper
    {
        public LinkageResponse Map(AddVerificationResponse addVerificationResponse)
        {
            if (addVerificationResponse == null)
            {
                throw new ArgumentNullException(nameof(addVerificationResponse));
            }
            
            var result = new LinkageResponse
            {
                LinkageKey = addVerificationResponse.LinkageKey,
                AccountId = addVerificationResponse.AccountId,
            };

            return result;
        }
    }
}