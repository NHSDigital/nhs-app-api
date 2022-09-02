using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public class ListServiceAccessesReply
    {
        [XmlElement("ServiceAccess")]
        public Collection<ServiceAccess> ServiceAccess { get; } = new Collection<ServiceAccess>();
    }
}