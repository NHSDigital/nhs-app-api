using System.Collections.Generic;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal interface IOrganDonationDonationWishesMapper : IMapper<DecisionDetails, Dictionary<string, string>>
    {
    }
}