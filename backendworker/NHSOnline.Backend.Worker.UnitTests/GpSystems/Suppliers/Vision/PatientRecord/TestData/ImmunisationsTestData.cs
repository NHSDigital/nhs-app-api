namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord.TestData
{
    public static class ImmunisationsTestData
    {
        private const string ResponseStart = "<root><patient>";
        private const string ResponseStringEnd = "</patient></root>";
        private const string InvalidResponseStart = "<rooot><patient>";
        private const string InvalidResponseStringEnd = "</patient></rooot>>";
        
        public static string GetInvalidImmunisationsTestData => $"{InvalidResponseStart}{InvalidResponseStringEnd}";
        
        public static string GetEmptyImmunisationsDataResponse => $"{ResponseStart}{GetInvalidImmunisationsXml()}{ResponseStringEnd}";
        
        public static string GetImmunisationsTestData => $"{ResponseStart}{GetImmunisationsXml()}{ResponseStringEnd}";

        private static string GetImmunisationsXml()
        {
            return
                "<clinical eventdate=\"2018-10-10T00:00:00\" read_term=\"Lumpectomy NEC\" subgroup_code=\"Immunisation\"/>";
        }
        private static string GetInvalidImmunisationsXml()
        {
            return
                "<clinical eventdate=\"2018-10-10T00:00:00\" read_term=\"Lumpectomy NEC\" subgroup_code=\"NOTImmunisation\"/>";
        }
    }
}