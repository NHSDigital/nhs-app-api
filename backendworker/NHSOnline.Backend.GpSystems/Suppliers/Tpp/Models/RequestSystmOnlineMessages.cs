using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class RequestSystmOnlineMessages : AbstractTppRequestModel
    {
        private RequestSystmOnlineMessages()
        {
        }

        public RequestSystmOnlineMessages(ITppUserSession userSession)
        {
            UnitId = userSession.UnitId;
            PatientId = userSession.PatientId;
            OnlineUserId = userSession.OnlineUserId;
        }
        
        [XmlAttribute("patientId")]
        public string PatientId { get; set; }
        
        [XmlAttribute("onlineUserId")]
        public string OnlineUserId { get; set; }

        [XmlIgnore]
        public override string RequestType => "RequestSystmOnlineMessages";
    }
}