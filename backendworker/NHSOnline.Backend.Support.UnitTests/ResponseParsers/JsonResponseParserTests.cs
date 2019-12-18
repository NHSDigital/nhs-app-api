using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.Support.UnitTests.ResponseParsers
{
    [TestClass]
    public class JsonResponseParserTests
    {
        [TestMethod]
        public void ParseBody_SuccessfulResponse_ReturnsDeserializedObject()
        {
            // Arrange
            var parser = new JsonResponseParser();
            var expected = new Application { DeviceType = "coco pops" };

            // Act
            var result = parser.ParseBody<Application>(
                "{ \"Name\" : null, \"Version\" : null, \"ProviderId\" : null, \"DeviceType\" : \"coco pops\" }"
            );

            // Assert
            result.Should().BeOfType<Application>();
            result?.DeviceType.Should().Be(expected.DeviceType);
        }

        [TestMethod]
        public void ParseBody_Failure()
        {
            var failing = "{" +
                          "\"MedicalRecord\": {" +
                          "\"PatientGuid\":\"00000000-0000-0000-0000-000000000000\"," +
                          "\"Title\":\"XXXXXXXXXX\"," +
                          "\"Forenames\":\"XXXXXXXXXX\"," +
                          "\"Surname\":\"XXXXXXXXXX\"," +
                          "\"Sex\":\"XXXXXXXXXX\"," +
                          "\"DateOfBirth\":\"0000-00-00T00:00:00\", " +
                          "\"Allergies\":null," +
                          "\"Consultations\":null," +
                          "\"Documents\":[" +
                          "{\"DocumentGuid\":\"00000000-0000-0000-0000-000000000000\",\"Observation\":{\"ObservationType\":\"Document\",\"Episodicity\":\"Unknown\",\"NumericValue\":null,\"NumericOperator\":null,\"NumericUnits\":null,\"DisplayValue\":null,\"TextValue\":null,\"Range\":null,\"Abnormal\":false,\"AbnormalReason\":null,\"AssociatedText\":null,\"EventGuid\":\"00000000-0000-0000-0000-000000000000\",\"Term\":\"XXXXXXXXXX\",\"AvailabilityDateTime\":\"2018-08-28T12:44:13.187\",\"EffectiveDate\":{\"DatePart\":\"YearMonthDay\",\"Value\":\"0000-00-00T00:00:00\"},\"CodeId\":00000000000000,\"AuthorisingUserInRoleGuid\":\"00000000-0000-0000-0000-000000000000\",\"EnteredByUserInRoleGuid\":\"00000000-0000-0000-0000-000000000000\"},\"Size\":00000, \"PageCount\":null,\"Extension\":\"rtf\", \"Available\":true}" +
                          "]," +
                          "\"Immunisations\":null, " +
                          "\"Medication\":null," +
                          "\"Problems\":null," +
                          "\"TestResults\":null, " +
                          "\"Users\":[ " +
                          "{\"UserInRoleGuid\":\"00000000-0000-0000-0000-000000000000\",\"Title\":\"XXXXXXXXXX\",\"Forenames\":\"XXXXXXXXXX\",\"Surname\":\"XXXXXXXXXX\",\"Role\":\"XXXXXXXXXX\",\"Organisation\":\"XXXXXXXXXX\"}" +
                          "] " +
                          "}," +
                          "\"FilterDetails\":{\"ItemFilter\":\"Documents\",\"ItemFilterFromDate\":null,\"ItemFilterToDate\":null,\"FreeTextFilterFromDate\":null}" +
                          "} ";

            var parser = new JsonResponseParser();

            // Act
            Action act = () => parser.ParseBody<MedicationRootObject>(failing);

            // Assert
            act.Should().Throw<NhsUnparsableException>()
                .And.ErrorMessages.Should().BeEquivalentTo(
                    new List<NhsUnparsableExceptionError>
                    {
                        new NhsUnparsableExceptionError(
                            "Could not convert string to DateTimeOffset",
                            "MedicalRecord.DateOfBirth"),
                        new NhsUnparsableExceptionError(
                            "Could not convert string to DateTime",
                            "MedicalRecord.Documents[0].Observation.EffectiveDate.Value")
                    });
        }

        [TestMethod]
        public void ParseBody_Failure_Invalid()
        {
            var failing =
                "{ \"Invalid\" }";

            var parser = new JsonResponseParser();

            // Act
            Action act = () => parser.ParseBody<Document>(failing);

            // Assert
            act.Should().Throw<NhsUnparsableException>()
                .And.ErrorMessages.Should().BeEquivalentTo(
                    new List<NhsUnparsableExceptionError>
                    {
                        new NhsUnparsableExceptionError(
                            "Invalid character after parsing property name.",
                            "")
                    });
        }



        [TestMethod]
        public void ParseBody_Failure_NotJson()
        {
            var failing = "<Application deviceType=\"coco pops\"></Application>";

            var parser = new JsonResponseParser();

            // Act
            Action act = () => parser.ParseBody<Application>(failing);

            // Assert
            act.Should().Throw<NhsUnparsableException>()
                .And.ErrorMessages.Should().BeEquivalentTo(
                    new List<NhsUnparsableExceptionError>
                    {
                        new NhsUnparsableExceptionError(
                            "Unexpected character encountered while parsing value",
                            "")
                    });
        }
    }
}