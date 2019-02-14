namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord
{
    public class Document
    {
        public string DocumentGuid { get; set; }
        public Observation Observation { get; set; }
        public int Size { get; set; }
        public int PageCount { get; set; }
        public string Extension { get; set; }
        public bool Available { get; set; }
    }
}