using System;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Linkage;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Linkage
{
    public class VisionLinkageMapper : IVisionLinkageMapper
    {
        public LinkageResponse Map(LinkageKeyPostResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var result = new LinkageResponse
            {
                LinkageKey = response.LinkageKey,
                AccountId = response.AccountId,
                OdsCode = response.OdsCode,
            };

            return result;
        }

        public LinkageResponse Map(LinkageKeyGetResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var result = new LinkageResponse
            {
                LinkageKey = response.LinkageKey,
                AccountId = response.AccountId,
                OdsCode = response.OdsCode,
            };

            return result;
        }
    }
}