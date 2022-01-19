using System;

namespace NHSOnline.App.Config
{
    public interface INhsExternalServicesConfiguration
    {
        Uri NhsUkBaseHelpUrl { get; }
        Uri NhsUkLoginHelpUrl { get; }
        Uri NhsUkLoginWhoCanUseTheAppHelpUrl { get; }
        Uri NhsUkHealthRecordDownloadHelpUrl { get; }
        Uri OneOneOneUrl { get; }
        Uri OneOneOneWalesUrl { get; }
        Uri NhsUkContactUsUrl { get; }
        Uri MyHealthOnlineUrl { get; }
        Uri CovidPassUrl { get; }
        Uri GpOutOfHoursService { get; }
        Uri CovidStatusService { get; }
        Uri NhsAppOnlineLogin { get; }
        Uri NhsAppTechnicalIssuesSupportUrl { get; }
    }
}