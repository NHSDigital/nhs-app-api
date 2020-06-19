using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Vision.Models
{
    public abstract class VisionResponseEnvelope
    {
        internal static VisionResponseEnvelope Create<T>(string name, string version, T serviceContent)
        {
            var visionResponseBody = new VisionResponseBody<T>(serviceContent);
            visionResponseBody.VisionResponse.ServiceDefinition.Name = name;
            visionResponseBody.VisionResponse.ServiceDefinition.Version = version;
            return new VisionResponseEnvelope<T>(visionResponseBody);
        }
    }

    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public sealed class VisionResponseEnvelope<T>: VisionResponseEnvelope
    {
        private VisionResponseEnvelope() => Body = null!;

        public VisionResponseEnvelope(VisionResponseBody<T> body) => Body = body;

        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public VisionResponseBody<T> Body { get; set; }
    }
}