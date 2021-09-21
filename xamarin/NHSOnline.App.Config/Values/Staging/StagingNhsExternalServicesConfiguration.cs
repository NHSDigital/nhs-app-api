using System;

namespace NHSOnline.App.Config.Values.Staging
{
    public class StagingNhsExternalServicesConfiguration: INhsExternalServicesConfiguration
    {
        public Uri NhsUkBaseHelpUrl { get; } = new Uri("https://www.nhs.uk/nhs-app/nhs-app-help-and-support/");
        public Uri NhsUkLoginHelpUrl { get; } = new Uri("https://www.nhs.uk/nhs-app/nhs-app-help-and-support/getting-started-with-the-nhs-app/");
        public Uri NhsUkLoginWhoCanUseTheAppHelpUrl { get;  } = new Uri("https://www.nhs.uk/nhs-app/nhs-app-help-and-support/getting-started-with-the-nhs-app/who-can-use-the-nhs-app/");
        public Uri OneOneOneUrl { get; } = new Uri("https://111.nhs.uk/");
        public Uri OneOneOneWalesUrl { get; } = new Uri("https://111.wales.nhs.uk/");
        public Uri NhsUkContactUsUrl { get; } = new Uri("https://www.nhs.uk/contact-us/nhs-app-contact-us");
        public Uri MyHealthOnlineUrl { get; } = new Uri("https://111.wales.nhs.uk/contactus/myhealthonline/");
    }
}