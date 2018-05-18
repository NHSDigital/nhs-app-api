namespace NHSOnline.Backend.Worker
{
    public class ConfigurationSettings
    {
        public int? PrescriptionsDefaultLastNumberMonthsToDisplay { get; set; }

        public int? PrescriptionsMaxCoursesSoftLimit { get; set; }

        public int? CoursesMaxCoursesLimit { get; set; }
    }
}