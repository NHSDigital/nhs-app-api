using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions.Models
{
    public class PrescriptionItem
    {
        public List<CourseEntry> Courses { get; set; }
        
        public DateTimeOffset? OrderDate { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Status? Status { get; set; }
    }
}
