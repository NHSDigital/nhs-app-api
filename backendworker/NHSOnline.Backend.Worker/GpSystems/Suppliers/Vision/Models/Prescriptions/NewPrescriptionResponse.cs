using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions
{
    public class OrderNewPrescriptionResponse
    {
        public const string OkResponseText = "OK";

        [XmlText]
        public string Result { get; set; }
    }
}
