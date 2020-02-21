using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData
{
    public class BinaryDataPage
    {
        [XmlText]
        public string BinaryData { get; set; }
    }
}