using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;
using static NHSOnline.Backend.Worker.Constants.OrganDonationConstants;

namespace NHSOnline.Backend.Worker.OrganDonation
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