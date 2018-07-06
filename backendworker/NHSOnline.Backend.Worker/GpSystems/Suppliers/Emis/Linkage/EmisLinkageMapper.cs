using System;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage
{
    public class EmisLinkageMapper : IEmisLinkageMapper
    {
        public LinkageResponse Map(LinkageDetailsResponse linkageDetailsResponse)
        {
            if (linkageDetailsResponse == null)
            {
                throw new ArgumentNullException(nameof(linkageDetailsResponse));
            }
            
            var result = new LinkageResponse
            {
                LinkageKey = linkageDetailsResponse.LinkageKey,
                OdsCode = linkageDetailsResponse.OdsCode,
                AccountId = linkageDetailsResponse.AccountId,
            };

            return result;
        }
    }
}