using Newtonsoft.Json;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions
{
    public class RequestedMedicationCourse
    {
        public string RequestedMedicationCourseGuid { get; set; }

        public RequestedMedicationCourseStatus RequestedMedicationCourseStatus { get; set; }
    }
}
