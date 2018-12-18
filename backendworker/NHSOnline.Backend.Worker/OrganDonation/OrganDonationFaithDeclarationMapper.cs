using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class OrganDonationFaithDeclarationMapper : EnumMapper<FaithDeclaration>
    {
        public OrganDonationFaithDeclarationMapper(ILogger<FaithDeclaration> logger) : base(logger)
        {
        }

        protected override Dictionary<string, FaithDeclaration> MappingTable => new Dictionary<string, FaithDeclaration>
        {
            { "yes", FaithDeclaration.Yes },
            { "no", FaithDeclaration.No },
            { "not-stated", FaithDeclaration.NotStated },
        };
    }
}