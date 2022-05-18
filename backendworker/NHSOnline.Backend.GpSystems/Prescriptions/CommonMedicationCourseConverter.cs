using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;

namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public static class CommonMedicationCourseConverter
    {
        public static string GroupAndConvertCommonMedicationCoursesToLogText(List<CommonMedicationCourse> medicationCourses)
        {
            var groupedMedicationCourses = GroupMedicationCourses(medicationCourses);
            var logText = CreateLogText(groupedMedicationCourses);
            return logText;
        }

        private static List<GroupedCommonMedicationCourse> GroupMedicationCourses(List<CommonMedicationCourse> medicationCourses)
        {
            var groupedMedicationCourses = medicationCourses
                .GroupBy(x => new { x.Type, x.Orderable })
                .Select(g => new GroupedCommonMedicationCourse()
                {
                    Type = g.Key.Type.ToString(),
                    Orderable = g.Key.Orderable,
                    Count = g.Count()
                })
                .OrderBy(x => x.Type)
                .ToList();

            return groupedMedicationCourses;
        }

        private static string CreateLogText(List<GroupedCommonMedicationCourse> groupedMedicationCourses)
        {
            var serializeMedicationDetails = Newtonsoft.Json.JsonConvert.SerializeObject(groupedMedicationCourses);
            var logText = $"courses_received_data = {serializeMedicationDetails}";
            return logText;
        }
    }
}