using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging
{
    public class Item
    {
        [XmlText]
        public string ItemText { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}