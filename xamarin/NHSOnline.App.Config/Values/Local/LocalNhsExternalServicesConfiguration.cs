using System;

namespace NHSOnline.App.Config.Values.Local
{
    public class LocalNhsExternalServicesConfiguration: INhsExternalServicesConfiguration
    {
        public Uri NhsUkCovidUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/covid");
        public Uri NhsUkCovidConditionsUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/covid/conditions");
        public Uri NhsUkLoginHelpUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/help/login");
        public Uri NhsUkConditionsUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/conditions");
        public Uri OneOneOneUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/111");
        public Uri NhsUkContactUsUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/contactus");
    }
}