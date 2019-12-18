using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.PfsApi.Areas.GpSearch.Models;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.Support.UnitTests.ResponseParsers
{
    [TestClass]
    public class XmlResponseParserTests
    {
        [TestMethod]
        public void ParseBody_SuccessfulResponse_ReturnsDeserializedObject()
        {
            // Arrange
            var parser = new XmlResponseParser();
            var expected = new Application { DeviceType = "coco pops" };
            
            // Act
            var result = parser.ParseBody<Application>("<Application deviceType=\"coco pops\"></Application>");

            // Assert
            result.Should().BeOfType<Application>();
            result?.DeviceType.Should().Be(expected.DeviceType);
        }

        [TestMethod]
        public void ParseBody_Failure_Invalid()
        {
            // Arrange
            var parser = new XmlResponseParser();

            var failing = "<Test SearchTerm=\"1\"></Test>";

            // Act
            Action act = () => parser.ParseBody<GpSearchRequest>(failing);

            // Assert
            act.Should().Throw<NhsUnparsableException>()
                .And.ErrorMessages.Should().BeEquivalentTo(
                    new List<NhsUnparsableExceptionError>
                    {
                        new NhsUnparsableExceptionError(
                            "There is an error in XML document",
                            "Unknown")
                    });
        }

        [TestMethod]
        public void ParseBody_Failure_StringToBoolean()
        {
            // Arrange
            var parser = new XmlResponseParser();

            var failing = "<ConsentResponse>" +
                          "<ConsentGiven>false</ConsentGiven>" +
                          "<AnalyticsCookieAccepted>false</AnalyticsCookieAccepted>" +
                          "<UpdatedConsentRequired>blah</UpdatedConsentRequired>" +
                          "</ConsentResponse>";

            // Act
            Action act = () => parser.ParseBody<ConsentResponse>(failing);

            // Assert
            act.Should().Throw<NhsUnparsableException>()
                .And.ErrorMessages.Should().BeEquivalentTo(
                    new List<NhsUnparsableExceptionError>
                    {
                        new NhsUnparsableExceptionError(
                            "The string '***' is not a valid Boolean value.",
                            "Unknown")
                    });
        }

        [TestMethod]
        public void ParseBody_Failure_StringToBooleanx()
        {
            // Arrange
            var parser = new XmlResponseParser();

            var failing = "<OrganDonationRegistration>" +
                          "<Identifier>test</Identifier>" +
                          "<NhsNumber>1234567890</NhsNumber>" +
                          "<NameFull>Miss Test</NameFull>" +
                          "<Name>Miss Test</Name>" +
                          "</OrganDonationRegistration>";

            // Act
            Action act = () => parser.ParseBody<OrganDonationRegistration>(failing);

            // Assert
            act.Should().Throw<NhsUnparsableException>()
                .And.ErrorMessages.Should().BeEquivalentTo(
                    new List<NhsUnparsableExceptionError>
                    {
                        new NhsUnparsableExceptionError(
                            "There was an error reflecting property 'DecisionDetails'.",
                            "Unknown")
                    });
        }
    }
}