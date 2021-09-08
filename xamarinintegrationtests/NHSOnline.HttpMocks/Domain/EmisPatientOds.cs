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
        Cie,
        SecondaryCareView,
        MyCareView,
        Substrakt,
        Gncr
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
                EmisPatientOds.NoOdsCode => "no-ods-code",
                EmisPatientOds.UnknownOdsCode => "Unknown",
                EmisPatientOds.Pkb => "emis_with_pkb",
                EmisPatientOds.Cie => "emis_with_cie",
                EmisPatientOds.SecondaryCareView => "emis_with_secondary_care_view",
                EmisPatientOds.MyCareView => "emis_with_my_care_view",
                EmisPatientOds.Substrakt => "emis_with_substrakt",
                EmisPatientOds.Gncr => "emis_with_gncr",
                _ => throw new ArgumentOutOfRangeException(nameof(odsCode), odsCode, null)
            };
        }
    }
}