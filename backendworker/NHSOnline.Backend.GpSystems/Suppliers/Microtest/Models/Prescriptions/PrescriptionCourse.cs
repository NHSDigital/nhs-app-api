using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions
{
    public class PrescriptionCourse
    {
        public string Id { get; set; }

        public DateTimeOffset? OrderDate { get; set; }

        public string Status { get; set; }
        
        public string Name { get; set; }

        public string Quantity { get; set; }

        public string Dosage { get; set; }

        public string Type { get; set; }

        public string Reason { get; set; }
    }
}
