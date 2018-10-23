using System;
using System.Globalization;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord.TestData
{
    public static class MedicationsTestData
    {
        private const string ResponseStart = "<root><patient>";
        private const string ResponseStringEnd = "</patient></root>";
        private const string InvalidResponseStart = "<rooot><patient>";
        private const string InvalidResponseStringEnd = "</patient></rooot>>";
        
        public static string GetInvalidMedicationTestData => $"{InvalidResponseStart}{InvalidResponseStringEnd}";
        
        public static string GetEmptyMedicationsDataResponse => $"{ResponseStart}{ResponseStringEnd}";
        
        public static string GetMedicationTestData(DateTime today)
        {      
            return $"{ResponseStart}{GetAcuteMedication(today)}{GetCurrentRepeatMedication(today)}{GetPastRepeatMedication(today)}{ResponseStringEnd}";
        }

        private static string GetAcuteMedication(DateTime now)
        {
            return   
                "<clinical dosage=\"FIVE TABLETS UP TO THREE TIMES DAILY AS REQUIRED\" " +
                "drug_term=\"Panadol ActiFast 500mg tablets (GlaxoSmithKline Consumer Healthcare)\" " +
                $"eventdate=\"{now.AddMonths(-10).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)}\" " +
                "packsize=\"tablets\" quantity=\"30\" subgroup_code=\"Acute\" />" +
                "<clinical dosage=\"SIX TABLETS UP TO TWICE DAILY AS REQUIRED\" " +
                "drug_term=\"Flucloxacillin 100mg capsules\" " +
                $"eventdate=\"{now.AddMonths(-13).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)}\" " +
                "packsize=\"capsules\" quantity=\"80\" subgroup_code=\"Acute\" />";
        }
        
        private static string GetPastRepeatMedication(DateTime now)
        {
            return
                "<clinical dosage=\"1 TO 2 TABLETS UP TO FOUR TIMES DAILY AS REQUIRED\" " +
                "drug_term=\"Flucloxacillin 250mg capsules\" " +
                "first_prescribed_date=\"2017-10-08T00:00:00\" " +
                $"last_prescribed_date=\"{now.AddMonths(-5).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)}\" " +
                "packsize=\"tablets\" quantity=\"28\" subgroup_code=\"DiscontinuedRepeat\" />" +
                "<clinical dosage=\"1 TABLET UP TO FIVE TIMES DAILY AS REQUIRED\" " +
                "drug_term=\"Panadol ActiFast 50mg tablets\" " +
                "first_prescribed_date=\"2012-10-08T01:01:01\" " +
                $"last_prescribed_date=\"{now.AddMonths(-7).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)}\" " +
                "packsize=\"cpasules\" quantity=\"14\" subgroup_code=\"DiscontinuedRepeat\" />";
        }
        
        private static string GetCurrentRepeatMedication(DateTime now)
        {
            return   
                "<clinical dosage=\"10 TABLETS UP TO TWO TIMES DAILY AS REQUIRED\" " +
                "drug_term=\"Panadol ActiFast 1000mg tablets (GlaxoSmithKline Consumer Healthcare)\" " +
                "first_prescribed_date=\"2015-10-08T01:01:01\" " +
                $"last_prescribed_date=\"{now.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)}\" " +
                "packsize=\"tablets\" quantity=\"45\" subgroup_code=\"CurrentRepeat\" />";
        }
    }
}