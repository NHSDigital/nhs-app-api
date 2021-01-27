using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public sealed class Registration
    {
        [XmlElement("PatientAccess")]
        public List<PatientAccess> PatientAccess { get; } = new List<PatientAccess>();
    }
}