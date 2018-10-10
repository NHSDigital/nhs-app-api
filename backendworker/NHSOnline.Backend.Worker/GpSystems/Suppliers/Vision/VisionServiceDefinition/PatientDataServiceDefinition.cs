namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.VisionServiceDefinition
{
    public class PatientDataServiceDefinition : IVisionServiceDefinition
    {
        public string Name => "VOS.GetPatientData";
        public string Version => "2.1.0";
    }
}
