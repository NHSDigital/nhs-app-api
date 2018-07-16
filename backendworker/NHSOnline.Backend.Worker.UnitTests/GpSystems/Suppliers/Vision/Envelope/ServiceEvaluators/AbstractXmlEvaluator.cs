using System.Xml;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public abstract class AbstractXmlEvaluator
{
    protected XPathNavigator nav;
    protected XmlNamespaceManager manager;

    protected const string soapBody = "boolean(//soapenv:Envelope/soapenv:Body/";

    public AbstractXmlEvaluator(string envelope)
    {
        XmlDocument d = new XmlDocument();
        d.LoadXml(envelope);
        nav = d.CreateNavigator();
        manager = new XmlNamespaceManager(d.NameTable);
        manager.AddNamespace("vision", "urn:vision");
        manager.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
        manager.AddNamespace("wsa", "http://www.w3.org/2005/08/addressing");
        manager.AddNamespace("wsse",
            "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
        manager.AddNamespace("wsu",
            "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
    }

    protected void ValidateServiceDefinition(string serviceDefinition, string serviceVersion)
    {
        Assert.IsTrue((bool) nav.Evaluate(
            soapBody + $"vision:visionRequest/vision:serviceDefinition/vision:name[text()='{serviceDefinition}'])",
            manager), $"Failed to find service definition: {serviceDefinition}");
        Assert.IsTrue((bool) nav.Evaluate(
            soapBody + $"vision:visionRequest/vision:serviceDefinition/vision:version[text()='{serviceVersion}'])",
            manager), $"Failed to find service version: {serviceVersion}");
    }
}