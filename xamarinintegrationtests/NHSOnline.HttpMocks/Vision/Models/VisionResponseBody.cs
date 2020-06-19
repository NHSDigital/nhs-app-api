using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class VisionResponseBody<T>
    {
        private VisionResponseBody() => VisionResponse = null!;

        public VisionResponseBody(T serviceContent) => VisionResponse = new VisionResponse<T>(serviceContent);

        [XmlElement(ElementName = "visionResponse", Namespace = "urn:vision")]
        public VisionResponse<T> VisionResponse { get; set; }
    }
}