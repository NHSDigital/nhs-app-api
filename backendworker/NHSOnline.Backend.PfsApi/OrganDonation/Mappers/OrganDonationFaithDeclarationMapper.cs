using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using static NHSOnline.Backend.Support.Constants.OrganDonationConstants;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal class OrganDonationFaithDeclarationMapper : OneToOneEnumMapper<FaithDeclaration>
    {
        public OrganDonationFaithDeclarationMapper(ILogger<FaithDeclaration> logger) : base(logger)
        {
        }

        protected override Dictionary<string, FaithDeclaration> MappingTable => new Dictionary<string, FaithDeclaration>
        {
            { YesChoiceValue, FaithDeclaration.Yes },
            { NoChoiceValue, FaithDeclaration.No },
            { NotStatedChoiceValue, FaithDeclaration.NotStated }
        };
    }
}
