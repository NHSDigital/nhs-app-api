using System;

namespace NHSOnline.App.Config.Values.Staging
{
    public class StagingBeforeYouStartConfiguration: IBeforeYouStartConfiguration
    {
        public Uri NhsUkCovidUrl { get; } = new Uri("http://stubs.local.bitraft.io/nhsuk/covid");
        public Uri NhsUkConditionsUrl { get; } = new Uri("http://stubs.local.bitraft.io/nhsuk/conditions");
        public Uri OneOneOneUrl { get; } = new Uri("http://stubs.local.bitraft.io/nhsuk/111");
    }
}