using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public interface IOrganDonationDataMaps
    {
        IDictionary<string, string> TitleDataMap { get; }
        
        IDictionary<string, string> GenderDataMap { get; }
    }
}