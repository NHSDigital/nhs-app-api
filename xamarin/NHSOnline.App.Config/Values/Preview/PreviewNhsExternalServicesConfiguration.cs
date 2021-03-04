using System;

namespace NHSOnline.App.Config.Values.Preview
{
    public class PreviewNhsExternalServicesConfiguration: INhsExternalServicesConfiguration
    {
        public Uri NhsUkCovidUrl { get; } = new Uri("https://111.nhs.uk/service/COVID-19/");
        public Uri NhsUkCovidAppUrl { get; } = new Uri("https://covid19.nhs.uk/");
        public Uri NhsUkCovidConditionsUrl { get; } = new Uri("https://www.nhs.uk/conditions/coronavirus-covid-19/");
        public Uri NhsUkBaseHelpUrl { get; } = new Uri("https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/");
        public Uri NhsUkLoginHelpUrl { get; } = new Uri("https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/app-login/");
        public Uri NhsUkConditionsUrl { get; } = new Uri("https://www.nhs.uk/conditions/");
        public Uri OneOneOneUrl { get; } = new Uri("https://111.nhs.uk/");
        public Uri OneOneOneWalesUrl { get; } = new Uri("https://111.wales.nhs.uk/");
        public Uri NhsUkContactUsUrl { get; } = new Uri("https://www.nhs.uk/contact-us/nhs-app-contact-us");
        public Uri MyHealthOnlineUrl { get; } = new Uri("https://111.wales.nhs.uk/contactus/myhealthonline/");
    }
}