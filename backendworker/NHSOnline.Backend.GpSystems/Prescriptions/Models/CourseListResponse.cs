using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NHSOnline.Backend.GpSystems.SharedModels;

namespace NHSOnline.Backend.GpSystems.Prescriptions.Models
{
    public class CourseListResponse
    {
        public IEnumerable<Course> Courses { get; set; } = new List<Course>();

        [JsonConverter(typeof(StringEnumConverter), false)]
        public Necessity SpecialRequestNecessity { get; set; } = Necessity.Optional;
    }
}
