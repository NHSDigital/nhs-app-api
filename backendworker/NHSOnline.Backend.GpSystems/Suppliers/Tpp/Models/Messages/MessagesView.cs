using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Messages
{
    [Serializable]
    public class MessagesView : AbstractTppRequestModel
    {
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }

        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlIgnore]
        public override string RequestType => "MessagesView";

    }
}