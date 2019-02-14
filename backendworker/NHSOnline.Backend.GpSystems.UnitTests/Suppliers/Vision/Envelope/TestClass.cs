using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Envelope
{
    public class TestClass
    {
        [XmlElement(ElementName = "hello", Namespace = "urn:vision")]
        public string TestProp { get; set; }        
    }
}