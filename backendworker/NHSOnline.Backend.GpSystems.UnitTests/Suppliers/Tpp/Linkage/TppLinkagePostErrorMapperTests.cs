using System;
using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Linkage
{
    [TestClass]
    public class TppLinkagePostErrorMapperTests
    {
        private IFixture _fixture;
        private Mock<ILogger<TppLinkageService>> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _logger = _fixture.Create<Mock<ILogger<TppLinkageService>>>();
        }

        [TestMethod]
        public void Map_WhenPassingNull_ThrowsNullReferenceException()
        {
            // Act
            Action act = () => TppLinkagePostErrorMapper.Map(null, _logger.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("response");
        }





        [DataTestMethod]
        [DataRow("6", Im1ConnectionErrorCodes.InternalCode.ProblemLinkingAccount)]
        [DataRow("8", Im1ConnectionErrorCodes.InternalCode.ProblemLinkingAccount)]
        [DataRow("19", Im1ConnectionErrorCodes.InternalCode.PatientOnSystemOneNotMatchedToARecordOnPDS)]
        [DataRow("509", Im1ConnectionErrorCodes.InternalCode.IncompleteOrEndedPFSRegistrationDetails)]
        [DataRow("512", Im1ConnectionErrorCodes.InternalCode.ProvidedLastNameDoesNotMatchSystmOne)]
        [DataRow("513", Im1ConnectionErrorCodes.InternalCode.ProvidedDOBDoesNotMatchSystmOne)]
        [DataRow("553", Im1ConnectionErrorCodes.InternalCode.PatientIsNotOldEnoughToSignUp)]
        [DataRow("554", Im1ConnectionErrorCodes.InternalCode.NoPatientWithNhsNumberExistsOnSystmOne)]
        [DataRow("555", Im1ConnectionErrorCodes.InternalCode.PatientNotRegisteredAtPracticeSpecifiedByOrgCode)]
        [DataRow("556", Im1ConnectionErrorCodes.InternalCode.ErrorCreatingNewPFSAccountAndLinkageKeys)]
        public void Map_WithOkayWithErrorCodeValues_MapsCorrectly(string errorCode, Im1ConnectionErrorCodes.InternalCode expectedError)
        {
            // Arrange
            var response = CreateResponse(
                HttpStatusCode.OK,
                errorCode);

            // Act
            var result = TppLinkagePostErrorMapper.Map(response, _logger.Object);

            // Assert
            result.Should().BeAssignableTo<LinkageResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(expectedError);
        }

        [TestMethod]
        public void Map_WithUnknownValues_MapsCorrectly()
        {
            // Arrange
            var response = CreateResponse(HttpStatusCode.OK, "10");

            // Act
            var result = TppLinkagePostErrorMapper.Map(response, _logger.Object);

            // Assert
            result.Should().BeAssignableTo<LinkageResult.UnmappedErrorWithStatusCode>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UnknownError);
        }

        private TppApiObjectResponse<LinkAccountReply> CreateResponse(
            HttpStatusCode statusCode,
            string errorCode)
        {
            var response = _fixture.Create<TppApiObjectResponse<LinkAccountReply>>();

            response.StatusCode = statusCode;
            response.ErrorResponse.ErrorCode = errorCode;
            return response;
        }
    }
}