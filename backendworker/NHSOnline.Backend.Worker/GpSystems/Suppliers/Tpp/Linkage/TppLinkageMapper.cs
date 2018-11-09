using System;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Linkage
{
    public class TppLinkageMapper : ITppLinkageMapper
    {
        public LinkageResponse Map(AddNhsUserRequest addNhsUserRequest, AddNhsUserResponse addNhsUserResponse)
        {
            if (addNhsUserRequest == null)
            {
                throw new ArgumentNullException(nameof(addNhsUserRequest));
            }
            
            if (addNhsUserResponse == null)
            {
                throw new ArgumentNullException(nameof(addNhsUserResponse));
            }

            return new LinkageResponse()
            {
                AccountId = addNhsUserResponse.AccountId,
                OdsCode = addNhsUserRequest.OrganisationCode,
                LinkageKey = Constants.TppConstants.LinkageKey

            };
        }
    }
}