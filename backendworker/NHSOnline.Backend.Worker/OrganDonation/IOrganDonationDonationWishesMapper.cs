using System.Collections.Generic;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal interface IOrganDonationDonationWishesMapper : IMapper<DecisionDetails, Dictionary<string, string>>
    {
    }
}