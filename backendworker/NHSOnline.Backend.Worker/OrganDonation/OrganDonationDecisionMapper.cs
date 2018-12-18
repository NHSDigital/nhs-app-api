using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class OrganDonationDecisionMapper : EnumMapper<Decision>
    {
        public OrganDonationDecisionMapper(ILogger<Decision> logger) : base(logger)
        {
        }

        protected override Dictionary<string, Decision> MappingTable => new Dictionary<string, Decision>
        {
            { "opt-in", Decision.OptIn },
            { "opt-out", Decision.OptOut }
        };
    }
}