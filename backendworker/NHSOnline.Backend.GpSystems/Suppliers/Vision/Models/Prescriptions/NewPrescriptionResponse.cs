using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions
{
    public class OrderNewPrescriptionResponse
    {
        public const string OkResponseText = "OK";

        [XmlText]
        public string Result { get; set; }
    }
}
