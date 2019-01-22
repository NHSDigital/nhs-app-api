using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;
using static NHSOnline.Backend.Worker.Constants.OrganDonationConstants;
using static NHSOnline.Backend.Worker.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.Worker.OrganDonation
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
                .IsNotNull(source, nameof(source), ThrowError)
                .IsNotNull(source?.Choices, nameof(source.Choices), ThrowError)
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