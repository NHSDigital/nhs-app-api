using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services
{
    public class ListServiceAccessesReply
    {
        [XmlElement("ServiceAccess")]
        public List<ServiceAccess> ServiceAccess { get; set; }
    }
}