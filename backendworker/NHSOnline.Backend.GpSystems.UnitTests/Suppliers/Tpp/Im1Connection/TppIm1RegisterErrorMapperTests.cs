using System;
using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Im1Connection
{
    [TestClass]
    public class TppIm1RegisterErrorMapperTests
    {
        private IFixture _fixture;
        private Mock<ILogger<TppIm1ConnectionService>> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _logger = _fixture.Create<Mock<ILogger<TppIm1ConnectionService>>>();
        }

        [TestMethod]
        public void Map_WhenPassingNull_ThrowsNullReferenceException()
        {
            // Act
            Action act = () => TppIm1RegisterErrorMapper.Map(null, _logger.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void Map_WithOkayWithUnknownErrorValues_MapsCorrectly()
        {
            // Arrange
            var response = CreateResponse(HttpStatusCode.OK, "10");

            // Act
            var result = TppIm1RegisterErrorMapper.Map(response, _logger.Object);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.UnmappedErrorWithStatusCode>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UnknownError);
        }

        [DataTestMethod]
        [DataRow("8", Im1ConnectionErrorCodes.InternalCode.InvalidLinkageDetailsTpp)]
        [DataRow("19", Im1ConnectionErrorCodes.InternalCode.PatientOnSystemOneNotMatchedToARecordOnPDS)]
        [DataRow("509", Im1ConnectionErrorCodes.InternalCode.IncompleteOrEndedPFSRegistrationDetails)]
        [DataRow("512", Im1ConnectionErrorCodes.InternalCode.ProvidedLastNameDoesNotMatchSystmOne)]
        [DataRow("513", Im1ConnectionErrorCodes.InternalCode.ProvidedDOBDoesNotMatchSystmOne)]
        [DataRow("553", Im1ConnectionErrorCodes.InternalCode.PatientIsNotOldEnoughToSignUp)]
        [DataRow("554", Im1ConnectionErrorCodes.InternalCode.NoPatientWithNhsNumberExistsOnSystmOne)]
        [DataRow("555", Im1ConnectionErrorCodes.InternalCode.PatientNotRegisteredAtPracticeSpecifiedByOrgCode)]
        [DataRow("556", Im1ConnectionErrorCodes.InternalCode.ErrorCreatingNewPFSAccountAndLinkageKeys)]
        public void Map_WithOkayWithTppErrorValues_MapsCorrectly(string errorCode, Im1ConnectionErrorCodes.InternalCode expectedError)
        {
            // Arrange
            var response = CreateResponse(HttpStatusCode.OK,
                errorCode);

            // Act
            var result = TppIm1RegisterErrorMapper.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(expectedError);
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