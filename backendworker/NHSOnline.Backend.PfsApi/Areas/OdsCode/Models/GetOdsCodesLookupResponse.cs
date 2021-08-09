using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.Areas.OdsCode.Models
{
    public class GetOdsCodesLookupResponse
    {
        public IEnumerable<string> OdsCodes { get; set; }
    }
}
