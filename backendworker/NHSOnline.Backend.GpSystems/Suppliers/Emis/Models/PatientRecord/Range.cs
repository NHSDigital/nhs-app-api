namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord
{
    public class Range
    {
        public double? Minimum { get; set; }
        public string MinimumText { get; set; }
        public double? Maximum { get; set; }
        public string MaximumText { get; set; }
        public string Units { get; set; }
    }
}