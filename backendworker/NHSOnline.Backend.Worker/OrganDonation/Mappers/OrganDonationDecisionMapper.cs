using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Worker.OrganDonation.Models;

namespace NHSOnline.Backend.Worker.OrganDonation.Mappers
{
    internal class OrganDonationDecisionMapper : OneToOneEnumMapper<Decision>
    {
        public OrganDonationDecisionMapper(ILogger<Decision> logger) : base(logger)
        {
        }

        protected override Dictionary<string, Decision> MappingTable => new Dictionary<string, Decision>
        {
            { "opt-in", Decision.OptIn },
            { "opt-out", Decision.OptOut },
            { "app-rep", Decision.AppRep }
        };
    }
}