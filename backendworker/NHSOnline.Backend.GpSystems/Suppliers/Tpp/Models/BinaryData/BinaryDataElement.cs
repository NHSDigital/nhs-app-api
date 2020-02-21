using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData
{
    public class BinaryDataElement
    {
        [XmlAttribute("fileType")]
        public string FileType { get; set; }

        [XmlElement("BinaryDataPage")]
        public BinaryDataPage BinaryDataPage { get; set; }
    }
}