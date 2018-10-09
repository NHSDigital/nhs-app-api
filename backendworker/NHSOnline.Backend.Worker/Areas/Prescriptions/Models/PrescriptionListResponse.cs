using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NHSOnline.Backend.Worker.Areas.SharedModels;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions.Models
{
    public class PrescriptionListResponse
    {
        public IEnumerable<PrescriptionItem> Prescriptions { get; set; }

        public IEnumerable<Course> Courses { get; set; }
    }
}
