using System;

namespace NHSOnline.App.Config
{
    public interface INhsExternalServicesConfiguration
    {
        Uri NhsUkCovidUrl { get; }
        Uri NhsUkCovidConditionsUrl { get; }
        Uri NhsUkLoginHelpUrl { get; }
        Uri NhsUkConditionsUrl { get; }
        Uri OneOneOneUrl { get; }
        Uri NhsUkContactUsUrl { get; }
    }
}