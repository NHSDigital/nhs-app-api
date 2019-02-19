using System.Collections.Generic;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Worker.OrganDonation.Models;

namespace NHSOnline.Backend.Worker.OrganDonation.Mappers
{
    internal interface IOrganDonationDonationWishesMapper : IMapper<DecisionDetails, Dictionary<string, string>>
    {
    }
}