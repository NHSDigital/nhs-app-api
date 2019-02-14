namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.PatientRecord.TestData
{
    public static class ProblemsTestData
    {
        private const string ResponseStart = "<root><patient>";
        private const string ResponseStringEnd = "</patient></root>";
        private const string InvalidResponseStart = "<rooot><patient>";
        private const string InvalidResponseStringEnd = "</patient></rooot>>";
        
        public static string GetInvalidProblemsTestData => $"{InvalidResponseStart}{InvalidResponseStringEnd}";
        
        public static string GetEmptyProblemsDataResponse => $"{ResponseStart}{ResponseStringEnd}";
        
        public static string GetProblemsTestData => $"{ResponseStart}{GetProblemsXml()}{ResponseStringEnd}";

        private static string GetProblemsXml()
        {
            return
                "<problems eventdate=\"2018-10-10T00:00:00\"  read_term=\"Peanut allergy\"  subgroup_code=\"PastProblem\"/> " +
                "<problems eventdate=\"2018-10-10T00:00:00\"  read_term=\"Broken leg\"  subgroup_code=\"CurrentProblem\"/> " +
                "<problems eventdate=\"2018-10-10T00:00:00\"  read_term=\"Acne\"  subgroup_code=\"Random\"/>";

        }
    }
}






