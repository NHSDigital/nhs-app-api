using System;

namespace NHSOnline.App.Config.Values.Scratch
{
    public class ScratchNhsExternalServicesConfiguration: INhsExternalServicesConfiguration
    {
        public Uri NhsUkBaseHelpUrl { get; } = new Uri("https://www.nhs.uk/nhs-app/nhs-app-help-and-support/");
        public Uri NhsUkLoginHelpUrl { get; } = new Uri("https://www.nhs.uk/nhs-app/nhs-app-help-and-support/getting-started-with-the-nhs-app/");
        public Uri NhsUkLoginWhoCanUseTheAppHelpUrl { get;  } = new Uri("https://www.nhs.uk/nhs-app/nhs-app-help-and-support/getting-started-with-the-nhs-app/who-can-use-the-nhs-app/");
        public Uri NhsUkHealthRecordDownloadHelpUrl { get; } = new Uri("https://www.nhs.uk/nhs-app/nhs-app-help-and-support/health-records-in-the-nhs-app/gp-health-record/");
        public Uri OneOneOneUrl { get; } = new Uri("https://111.nhs.uk/");
        public Uri DigitalCovidPassUrl { get; } = new Uri("https://covid-status.service.nhsx.nhs.uk/");
        public Uri PaperCovidPassUrl { get; } = new Uri("https://www.nhs.uk/conditions/coronavirus-covid-19/covid-pass/get-your-covid-pass-letter/");
        public Uri OneOneOneWalesUrl { get; } = new Uri("https://111.wales.nhs.uk/");
        public Uri NhsUkContactUsUrl { get; } = new Uri("https://www.nhs.uk/contact-us/nhs-app-contact-us");
        public Uri MyHealthOnlineUrl { get; } = new Uri("https://111.wales.nhs.uk/contactus/myhealthonline/");
        public Uri CovidPassUrl { get; } = new Uri("https://www.nhs.uk/conditions/coronavirus-covid-19/covid-pass/");
        public Uri GpOutOfHoursService { get; } = new Uri("https://www.nidirect.gov.uk/articles/gp-out-hours-service");
        public Uri CovidStatusService { get; } = new Uri("https://covid-status.service.nhsx.nhs.uk");
        public Uri NhsAppOnlineLogin { get; } = new Uri("https://www.nhsapp.service.nhs.uk/login");
        public Uri NhsAppTechnicalIssuesSupportUrl { get; } = new Uri("https://www.nhs.uk/nhs-app/nhs-app-help-and-support/nhs-app-technical-information/technical-issues-with-the-nhs-app/");
        public Uri NhsAppGetDocumentDownloadHelpUrl { get; } = new Uri("https://www.nhs.uk/nhs-app/nhs-app-help-and-support/health-records-in-the-nhs-app/gp-health-record/");
    }
}
