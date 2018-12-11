using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Linkage;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Linkage
{
    public class CreateLinkageKey
    {
        public string OdsCode { get; set; }

        public LinkageKeyPostRequest LinkageKeyPostRequest { get; set; }
    }
}
