using System;

namespace NHSOnline.App.Config.Values.Scratch18
{
    public class Scratch18NhsExternalServicesConfiguration: INhsExternalServicesConfiguration
    {
        public Uri NhsUkCovidUrl { get; } = new Uri("https://111.nhs.uk/service/COVID-19/");
        public Uri NhsUkCovidConditionsUrl { get; } = new Uri("https://www.nhs.uk/conditions/coronavirus-covid-19/");
        public Uri NhsUkBaseHelpUrl { get; } = new Uri("https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/");
        public Uri NhsUkLoginHelpUrl { get; } = new Uri("https://www.nhs.uk/nhs-app/nhs-app-help-and-support/getting-started-with-the-nhs-app/logging-in-to-the-nhs-app/");
        public Uri NhsUkTechnicalIssuesHelpUrl { get; } = new Uri("https://www.nhs.uk/nhs-app/nhs-app-help-and-support/nhs-app-technical-information/technical-issues-with-the-nhs-app/");
        public Uri NhsUkConditionsUrl { get; } = new Uri("https://www.nhs.uk/conditions/");
        public Uri OneOneOneUrl { get; } = new Uri("https://111.nhs.uk/");
        public Uri OneOneOneWalesUrl { get; } = new Uri("https://111.wales.nhs.uk/");
        public Uri NhsUkContactUsUrl { get; } = new Uri("https://www.nhs.uk/contact-us/nhs-app-contact-us");
        public Uri MyHealthOnlineUrl { get; } = new Uri("https://111.wales.nhs.uk/contactus/myhealthonline/");
    }
}