using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;

namespace NHSOnline.Backend.PfsApi.UnitTests.Prescriptions
{
    [TestClass]
    public class CommonMedicationCourseConverterTests
    {
        [TestMethod]
        public void GroupAndConvertCommonMedicationCoursesToLogText_ReturnsCorrectStringToLog()
        {
            var medicationCourses = new List<CommonMedicationCourse>()
            {
                new CommonMedicationCourse()
                {
                    Type = PrescriptionType.Repeat.ToString(),
                    Orderable = "yes"
                },
                new CommonMedicationCourse()
                {
                    Type = PrescriptionType.Repeat.ToString(),
                    Orderable = "yes"
                },
                new CommonMedicationCourse()
                {
                    Type = PrescriptionType.Repeat.ToString(),
                    Orderable = "yes"
                },
                new CommonMedicationCourse()
                {
                    Type = PrescriptionType.Repeat.ToString(),
                    Orderable = "no"
                },
                new CommonMedicationCourse()
                {
                    Type = PrescriptionType.Repeat.ToString(),
                    Orderable = "no"
                },

                new CommonMedicationCourse()
                {
                    Type = PrescriptionType.Acute.ToString(),
                    Orderable = "yes"
                },
                new CommonMedicationCourse()
                {
                    Type = PrescriptionType.Acute.ToString(),
                    Orderable = "yes"
                },
                new CommonMedicationCourse()
                {
                    Type = PrescriptionType.Acute.ToString(),
                    Orderable = "no"
                },

                new CommonMedicationCourse()
                {
                    Type = PrescriptionType.RepeatDispensing.ToString(),
                    Orderable = "yes"
                },
                new CommonMedicationCourse()
                {
                    Type = PrescriptionType.RepeatDispensing.ToString(),
                    Orderable = "no"
                },

                new CommonMedicationCourse()
                {
                    Type = PrescriptionType.Unknown.ToString(),
                    Orderable = "yes"
                },

                new CommonMedicationCourse()
                {
                    Type = PrescriptionType.Automatic.ToString(),
                    Orderable = "null"
                },
            };

            var result = CommonMedicationCourseConverter.GroupAndConvertCommonMedicationCoursesToLogText(medicationCourses);

            var expected = @"courses_received_data = [{""Type"":""Acute"",""Orderable"":""yes"",""Count"":2},{""Type"":""Acute"",""Orderable"":""no"",""Count"":1},{""Type"":""Automatic"",""Orderable"":""null"",""Count"":1},{""Type"":""Repeat"",""Orderable"":""yes"",""Count"":3},{""Type"":""Repeat"",""Orderable"":""no"",""Count"":2},{""Type"":""RepeatDispensing"",""Orderable"":""yes"",""Count"":1},{""Type"":""RepeatDispensing"",""Orderable"":""no"",""Count"":1},{""Type"":""Unknown"",""Orderable"":""yes"",""Count"":1}]";

            result.Should().Be(expected);
        }
    }
}