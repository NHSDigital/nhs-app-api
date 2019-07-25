using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Mappers
{
    public interface IOlcDataMaps
    {
        IDictionary<string, string> TitleDataMap { get; }
    }
}