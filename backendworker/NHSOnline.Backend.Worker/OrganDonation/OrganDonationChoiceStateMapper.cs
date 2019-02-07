using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;
using static NHSOnline.Backend.Worker.Constants.OrganDonationConstants;

namespace NHSOnline.Backend.Worker.OrganDonation
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