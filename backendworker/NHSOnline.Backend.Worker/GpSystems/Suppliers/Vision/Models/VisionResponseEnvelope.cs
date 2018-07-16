using System.Text;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class VisionResponseEnvelope<T>
    {
        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public VisionResponseBody<T> Body { get; set; }

        [XmlAttribute(AttributeName = "soap", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Soap { get; set; }

        public string GetErrorFormatted()
        {
            var errorText = new StringBuilder();

            var fault = Body?.Fault;

            if (fault != null)
            {
                errorText.Append($"Fault code: {fault.FaultCode}; Fault string:{fault.FaultString};");

                var visionFaultError = fault.Detail?.VisionFault?.Error;

                if (visionFaultError != null)
                {
                    errorText.Append($" Category: {visionFaultError.Category}; Code: {visionFaultError.Code}; Text: {visionFaultError.Text}");
                }
            }

            return errorText.ToString();
        }
    }
}
