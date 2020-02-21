using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData
{
    public class RequestBinaryData: AbstractTppRequestModel
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlAttribute("binaryDataId")]
        public string BinaryDataId { get; set; }

        [XmlIgnore]
        public override string RequestType => "RequestBinaryData";
    }
}