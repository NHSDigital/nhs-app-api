using System;

namespace NHSOnline.HttpMocks.Domain
{
    public enum EmisPatientOds
    {
        PkbAndEconsult,
        NotificationsPromptEnabled,
        AllSilversEnabled,
        NoOdsCode,
        UnknownOdsCode,
        Pkb,
        Substrakt,
        Gncr,
        Accurx,
        Nbs
    }

    public static class EmisPatientOdsExtensions
    {
        public static string ToOdsCodeString(this EmisPatientOds odsCode)
        {
            return odsCode switch
            {
                EmisPatientOds.NotificationsPromptEnabled => "emis_with_notifications_prompt_enabled",
                EmisPatientOds.PkbAndEconsult => "emis_with_pkb_and_econsult",
                EmisPatientOds.AllSilversEnabled => "emis_with_all_silvers",
                EmisPatientOds.NoOdsCode => string.Empty,
                EmisPatientOds.UnknownOdsCode => "Unknown",
                EmisPatientOds.Pkb => "emis_with_pkb",
                EmisPatientOds.Substrakt => "emis_with_substrakt",
                EmisPatientOds.Gncr => "emis_with_gncr",
                EmisPatientOds.Accurx => "emis_with_accurx",
                EmisPatientOds.Nbs => "emis_with_nbs",
                _ => throw new ArgumentOutOfRangeException(nameof(odsCode), odsCode, null)
            };
        }
    }
}