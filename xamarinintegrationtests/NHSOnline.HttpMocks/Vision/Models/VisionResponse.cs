using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    [XmlRoot(ElementName = "visionResponse", Namespace = "urn:vision")]
    public class VisionResponse<T>
    {
        private VisionResponse() => ServiceContent = default!;

        public VisionResponse(T serviceContent) => ServiceContent = serviceContent;

        [XmlElement(ElementName = "serviceDefinition", Namespace = "urn:vision")]
        public ServiceDefinition ServiceDefinition { get; set; } = new ServiceDefinition();

        [XmlElement(ElementName = "serviceHeader", Namespace = "urn:vision")]
        public ServiceHeaderResponse ServiceHeader { get; set; } = new ServiceHeaderResponse();

        [XmlElement(ElementName = "serviceContent", Namespace = "urn:vision")]
        public T ServiceContent { get; set; }

        [XmlAttribute(AttributeName = "vision", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Vision => "Vision";
    }
}