using System;
using System.Globalization;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models
{
    [XmlRoot(ElementName = "visionRequest", Namespace = "urn:vision")]
    public class VisionRequest<T>
    {
        [XmlElement(ElementName = "serviceDefinition", Namespace = "urn:vision")]
        public ServiceDefinition ServiceDefinition { get; set; }

        [XmlElement(ElementName = "serviceHeader", Namespace = "urn:vision")]
        public ServiceHeader ServiceHeader { get; set; }

        [XmlElement(ElementName = "serviceContent", Namespace = "urn:vision")]
        public ServiceContent<T> ServiceContent { get; set; }

        [XmlAttribute(AttributeName = "vision", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Vision { get; set; }

        public VisionRequest(string serviceName, string serviceVersion, string rosuAccountId, string apiKey,
            string odsCode, string providerId, T serviceContent)
        {
            ServiceDefinition = new ServiceDefinition
            {
                Name = serviceName,
                Version = serviceVersion
            };
            ServiceHeader = new ServiceHeader
            {
                CreationTime = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture),
                Credentials = new Credentials
                {
                    ApiKey = apiKey,
                    RosuAccountId = rosuAccountId
                },
                Id = Guid.NewGuid().ToString(),
                OpsReference = new OpsReference
                {
                    // Accountid and provider will always be the same value
                    AccountId = providerId,
                    Provider = providerId
                },
                Target = new Target
                {
                    NationalCode = odsCode
                },
                Xmlns = "Vision"
            };
            
            ServiceContent = new ServiceContent<T>
            {
                ServiceContentBody = serviceContent
            };
        }

        public VisionRequest() { }
    }
}
