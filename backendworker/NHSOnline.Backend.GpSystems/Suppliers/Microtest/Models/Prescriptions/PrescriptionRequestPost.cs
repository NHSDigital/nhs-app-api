using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions
{
    public class PrescriptionRequestsPost
    {
        [JsonProperty("courseIds")]
        public IEnumerable<string> CourseIds { get; set; }

        [JsonProperty("specialRequestMessage")]
        public string SpecialRequestMessage { get; set; }
    }
}
