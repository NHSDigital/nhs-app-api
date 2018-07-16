using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Envelope
{
    public class TestClass
    {
        [XmlElement(ElementName = "hello", Namespace = "urn:vision")]
        public string TestProp { get; set; }        
    }
}