using System;

namespace NHSOnline.App.Config.Values.Local
{
    public class LocalNhsExternalServicesConfiguration: INhsExternalServicesConfiguration
    {
        public Uri NhsUkBaseHelpUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/help/home");
        public Uri NhsUkLoginHelpUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/help-and-support/logging-in-to-the-nhs-app/");
        public Uri NhsUkLoginWhoCanUseTheAppHelpUrl { get;  } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/help-and-support/who-can-use-the-nhs-app/");
        public Uri NhsUkHealthRecordDownloadHelpUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/help/home");
        public Uri OneOneOneUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/111");
        public Uri OneOneOneWalesUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/111wales");
        public Uri NhsUkContactUsUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/contactus");
        public Uri MyHealthOnlineUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/myhealthonline");
        public Uri CovidPassUrl { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/covid-pass/");
        public Uri GpOutOfHoursService { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/gp-out-hours-service");
        public Uri CovidStatusService { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/covid-status-service-nhsx-nhs-uk");
        public Uri NhsAppOnlineLogin { get; } = new Uri("http://stubs.local.bitraft.io:8080/nhsuk/login");
    }
}