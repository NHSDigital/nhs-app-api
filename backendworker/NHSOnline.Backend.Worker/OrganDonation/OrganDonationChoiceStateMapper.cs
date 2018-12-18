using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class OrganDonationChoiceStateMapper : EnumMapper<ChoiceState>
    {
        public OrganDonationChoiceStateMapper(ILogger<ChoiceState> logger) : base(logger)
        {
        }

        protected override Dictionary<string, ChoiceState> MappingTable => new Dictionary<string, ChoiceState>
        {
            { "yes", ChoiceState.Yes },
            { "no", ChoiceState.No },
            { "not-stated", ChoiceState.NotStated },
        };
    }
}