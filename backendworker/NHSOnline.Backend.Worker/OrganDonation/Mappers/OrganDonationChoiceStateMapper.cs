using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using static NHSOnline.Backend.Support.Constants.OrganDonationConstants;

namespace NHSOnline.Backend.Worker.OrganDonation.Mappers
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