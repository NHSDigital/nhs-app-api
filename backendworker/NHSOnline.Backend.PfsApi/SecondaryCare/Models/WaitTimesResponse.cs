using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Models
{
    public class WaitTimesResponse
    {
        public List<SecondaryCareWaitTimeItem> WaitTimes { get; set; } = new List<SecondaryCareWaitTimeItem>();
    }
}