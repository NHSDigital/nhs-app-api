using System.Collections.Generic;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal interface IOrganDonationDonationWishesMapper : IMapper<DecisionDetails, Dictionary<string, string>>
    {
    }
}