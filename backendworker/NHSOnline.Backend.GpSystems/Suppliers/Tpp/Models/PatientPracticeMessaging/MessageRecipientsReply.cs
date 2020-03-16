using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging
{
    public class MessageRecipientsReply
    {
        [XmlElement("Item")]
        public List<Item> Items { get; set; }
    }
}