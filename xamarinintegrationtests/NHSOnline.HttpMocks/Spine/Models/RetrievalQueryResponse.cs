using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class RetrievalQueryResponse
    {
        [XmlElement(ElementName="QUPA_IN000009UK03", Namespace="urn:hl7-org:v3", IsNullable = true)]
        public QUPAIN000009UK03? QUPAIN000009UK03 { get; set; }
    }
}