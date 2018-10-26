namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord.TestData
{
    public static class AllergiesTestData
    {
        private const string ResponseStart = "<root><patient>";
        private const string ResponseStringEnd = "</patient></root>";
        private const string InvalidResponseStart = "<rooot><patient>";
        private const string InvalidResponseStringEnd = "</patient></rooot>>";
        
        public static string GetInvalidAllergiesTestData => $"{InvalidResponseStart}{InvalidResponseStringEnd}";
        
        public static string GetEmptyAllergiesDataResponse => $"{ResponseStart}{ResponseStringEnd}";

        public static string GetAllergiesTestData => $"{ResponseStart}{GetAllergiesTestDataXml()}{ResponseStringEnd}";

        private static string GetAllergiesTestDataXml()
        {
            return
                "<clinical eventdate=\"2007-05-12T00:00:00\" drug_term=\"Paracetamol 500mg capsules\" " +
                "read_code=\"14L..00\" read_term=\"H/O: drug allergy\" read_code2=\"1833.00\" " +
                "read_term2=\"Leg swelling\"/> " +
                "<clinical eventdate=\"2018-07-09T00:00:00\" read_code=\"SN58911\" read_term=\"Strawberry allergy\"/>";
        }
    }
}