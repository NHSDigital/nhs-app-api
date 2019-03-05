using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public interface IOrganDonationDataMaps
    {
        IDictionary<string, string> TitleDataMap { get; }
        
        IDictionary<string, string> GenderDataMap { get; }
    }
}