using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.Constants.OrganDonationConstants;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    public class OrganDonationChoiceStateMapper : OneToOneEnumMapper<ChoiceState>
    {
        public OrganDonationChoiceStateMapper(ILogger<ChoiceState> logger) : base(logger)
        {
        }

        protected override Dictionary<string, ChoiceState> MappingTable => new Dictionary<string, ChoiceState>
        {
            { YesChoiceValue, ChoiceState.Yes },
            { NoChoiceValue, ChoiceState.No },
            { NotStatedChoiceValue, ChoiceState.NotStated }
        };
    }
}
