using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class AllergyListResponse
    {
        public IEnumerable<AllergyItem> Allergies { get; set; }
    }
}
