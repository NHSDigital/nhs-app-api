using System;

namespace NHSOnline.App.Config
{
    public interface INhsExternalServicesConfiguration
    {
        Uri NhsUkCovidUrl { get; }
        Uri NhsUkCovidAppUrl { get; }
        Uri NhsUkCovidConditionsUrl { get; }
        Uri NhsUkBaseHelpUrl { get; }
        Uri NhsUkLoginHelpUrl { get; }
        Uri NhsUkConditionsUrl { get; }
        Uri OneOneOneUrl { get; }
        Uri OneOneOneWalesUrl { get; }
        Uri NhsUkContactUsUrl { get; }
        Uri MyHealthOnlineUrl { get; }
    }
}