using System;

namespace NHSOnline.HttpMocks.Domain
{
    public enum WayfinderPatientOds
    {
        ACCURX,
        DRDOCTOR,
        ERS,
        ERSANDPKB,
        GNCR,
        HEALTHCARECOMMS,
        PKB,
        NETCALL,
        ZESTY,
    }

    public static class WayfinderPatientOdsExtensions
    {
        public static string ToOdsCodeString(this WayfinderPatientOds odsCode)
        {
            return odsCode switch
            {
                WayfinderPatientOds.ACCURX => "wayfinder_with_accurx_enabled",
                WayfinderPatientOds.ERS => "wayfinder_with_ers_enabled",
                WayfinderPatientOds.GNCR => "wayfinder_with_gncr_enabled",
                WayfinderPatientOds.PKB => "wayfinder_with_pkb_enabled",
                WayfinderPatientOds.ERSANDPKB => "wayfinder_with_ers_and_pkb_enabled",
                WayfinderPatientOds.DRDOCTOR => "wayfinder_with_drDoctor_enabled",

                _ => throw new ArgumentOutOfRangeException(nameof(odsCode), odsCode, null)
            };
        }
    }
}
