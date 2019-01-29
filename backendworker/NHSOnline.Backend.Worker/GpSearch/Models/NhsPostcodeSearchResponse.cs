using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.Backend.Worker.GpSearch.Models
{
    public class NhsPostcodeSearchResponse
    {
        [JsonProperty(PropertyName = "value")]
        public List<PostcodeData> PostcodeDatas { get; set; }
    }
}