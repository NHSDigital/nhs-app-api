using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using static NHSOnline.Backend.Support.Constants.OrganDonationConstants;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal class OrganDonationDonationWishesMapper : IOrganDonationDonationWishesMapper
    {
        private readonly IEnumMapper<string, ChoiceState> _choiceStateMapper;
        private readonly ILogger<OrganDonationDonationWishesMapper> _logger;

        public OrganDonationDonationWishesMapper(IEnumMapper<string, ChoiceState> choiceStateMapper,
            ILogger<OrganDonationDonationWishesMapper> logger)
        {
            _choiceStateMapper = choiceStateMapper;
            _logger = logger;
        }

        public Dictionary<string, string> Map(DecisionDetails source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ValidateAndLog.ValidationOptions.ThrowError)
                .IsNotNull(source?.Choices, nameof(source.Choices), ValidateAndLog.ValidationOptions.ThrowError)
                .IsValid();
                
            var donationWishes = new Dictionary<string, string>
            {
                { AllOrgansChoiceKey, source.All ? YesChoiceValue : NoChoiceValue }
            };

            source.Choices.ToList().ForEach(c => donationWishes.Add(c.Key, _choiceStateMapper.From(c.Value)));

            return donationWishes;
        }
    }
}