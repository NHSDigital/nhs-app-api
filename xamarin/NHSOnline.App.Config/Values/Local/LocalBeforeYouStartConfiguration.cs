using System;

namespace NHSOnline.App.Config.Values.Local
{
    public class LocalBeforeYouStartConfiguration: IBeforeYouStartConfiguration
    {
        public Uri NhsUkCovidUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/covid");
        public Uri NhsUkConditionsUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/conditions");
        public Uri OneOneOneUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/111");
    }
}