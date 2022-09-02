using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    [Serializable]
    public sealed class Registration
    {
        [XmlElement("PatientAccess")]
        public Collection<PatientAccess> PatientAccess { get; } = new Collection<PatientAccess>();
    }
}