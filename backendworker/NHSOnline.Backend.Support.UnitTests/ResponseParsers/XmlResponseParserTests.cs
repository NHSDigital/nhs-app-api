using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.Support.UnitTests.ResponseParsers
{
    [TestClass]
    public class XmlResponseParserTests
    {
        [TestMethod]
        public void ParseBody_SuccessfulResponse_ReturnsDeserialzedObject()
        {
            var parser = new XmlResponseParser();
            var expected = new Application { DeviceType = "coco pops" };
            
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            var result = parser.ParseBody<Application>(
                "<Application deviceType=\"coco pops\"></Application>", 
                httpResponse
            );

            result.Should().BeOfType<Application>();
            result?.DeviceType.Should().Be(expected.DeviceType);
        }
    }
}