using System;

namespace NHSOnline.App.Config
{
    public interface INhsExternalServicesConfiguration
    {
        Uri NhsUkBaseHelpUrl { get; }
        Uri NhsUkLoginHelpUrl { get; }
        Uri NhsUkLoginWhoCanUseTheAppHelpUrl { get; }
        Uri OneOneOneUrl { get; }
        Uri OneOneOneWalesUrl { get; }
        Uri NhsUkContactUsUrl { get; }
        Uri MyHealthOnlineUrl { get; }
    }
}