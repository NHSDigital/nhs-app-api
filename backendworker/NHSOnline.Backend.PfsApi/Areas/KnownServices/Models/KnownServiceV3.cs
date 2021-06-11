using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.Areas.KnownServices.Models
{
    public class KnownServiceV3
    {
        public string Id { get; set; }

        public Uri Url { get; set; }

        public bool ShowThirdPartyWarning{ get; set; }

        public bool RequiresAssertedLoginIdentity { get; set; }

        public List<Uri> Domains { get; set; }
    }
}