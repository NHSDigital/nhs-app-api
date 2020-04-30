using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.Areas.Configuration.Models
{
    public class RootService: KnownService
    {
        public string Id { get; set; }
        public Uri Url { get; set; }

        public List<SubService> SubServices { get; set; }
    }
}