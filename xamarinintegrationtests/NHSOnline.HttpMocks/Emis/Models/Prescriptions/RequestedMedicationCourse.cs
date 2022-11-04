namespace NHSOnline.HttpMocks.Emis.Models.Prescriptions
{
    public class RequestedMedicationCourse
    {
        public string? RequestedMedicationCourseGuid { get; set; }

        public RequestedMedicationCourseStatus RequestedMedicationCourseStatus { get; set; }
    }
}