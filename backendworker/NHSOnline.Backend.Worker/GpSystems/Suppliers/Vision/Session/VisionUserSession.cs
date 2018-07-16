namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session
{
    public class VisionUserSession : UserSession
    {
        public override SupplierEnum Supplier => SupplierEnum.Vision;

        public string RosuAccountId { get; set; }
        public string OdsCode { get; set; }
        public string ApiKey { get; set; }
    }
}
