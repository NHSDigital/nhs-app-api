using System.Xml;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Envelope.ServiceEvaluators
{
    public abstract class AbstractXmlEvaluator
    {
        protected readonly XPathNavigator Nav;
        protected readonly XmlNamespaceManager Manager;

        protected const string SoapBody = "boolean(//soapenv:Envelope/soapenv:Body/";

        protected AbstractXmlEvaluator(string envelope)
        {
            XmlDocument d = new XmlDocument();
            d.LoadXml(envelope);
            Nav = d.CreateNavigator();
            Manager = new XmlNamespaceManager(d.NameTable);
            Manager.AddNamespace("vision", "urn:vision");
            Manager.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            Manager.AddNamespace("wsa", "http://www.w3.org/2005/08/addressing");
            Manager.AddNamespace("wsse",
                "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            Manager.AddNamespace("wsu",
                "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
        }

        protected void ValidateServiceDefinition(string serviceDefinition, string serviceVersion)
        {
            Assert.IsTrue((bool) Nav.Evaluate(
                SoapBody + $"vision:visionRequest/vision:serviceDefinition/vision:name[text()='{serviceDefinition}'])",
                Manager), $"Failed to find service definition: {serviceDefinition}");
            Assert.IsTrue((bool) Nav.Evaluate(
                SoapBody + $"vision:visionRequest/vision:serviceDefinition/vision:version[text()='{serviceVersion}'])",
                Manager), $"Failed to find service version: {serviceVersion}");
        }
    }
}
