using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Linkage
{
    [TestClass]
    public class EmisLinkageServiceTests
    {
        private EmisLinkageService _systemUnderTest;
        private Mock<IEmisClient> _emisClient;
        private Mock<IEmisLinkageMapper> _emisLinkageMapper;
        private IFixture _fixture;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _emisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _emisLinkageMapper = _fixture.Freeze<Mock<IEmisLinkageMapper>>();
            _systemUnderTest = _fixture.Create<EmisLinkageService>();
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            var linkageDetailsResponse = _fixture.Create<LinkageDetailsResponse>();
            var nhsNumber = _fixture.Create<string>();
            var odsCode = _fixture.Create<string>();

            _emisClient.Setup(x => x.LinkageGet(nhsNumber, odsCode))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<LinkageDetailsResponse>(HttpStatusCode.OK)
                    {
                        Body = linkageDetailsResponse,
                    }));

            // Act
            var result = await _systemUnderTest.GetLinkageKey(nhsNumber, odsCode);

            // Assert
            _emisClient.Verify(x => x.LinkageGet(nhsNumber, odsCode));
            _emisLinkageMapper.Verify(x => x.Map(linkageDetailsResponse));
            result.Should().BeAssignableTo<GetLinkageResult.SuccessfullyRetrieved>();
            ((GetLinkageResult.SuccessfullyRetrieved) result).Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Get_ReturnsNhsNumberNotFoundError_WhenEmisRespondsWith404()
        {
            // Arrange
            var nhsNumber = _fixture.Create<string>();
            var odsCode = _fixture.Create<string>();

            _emisClient.Setup(x => x.LinkageGet(nhsNumber, odsCode))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<LinkageDetailsResponse>(HttpStatusCode.NotFound)));

            // Act
            var result = await _systemUnderTest.GetLinkageKey(nhsNumber, odsCode);

            // Assert
            _emisClient.Verify(x => x.LinkageGet(nhsNumber, odsCode));
            result.Should().BeAssignableTo<GetLinkageResult.NhsNumberNotFound>();
        }
        
        [TestMethod]
        public async Task Get_ReturnsLinkageKeyRevokedError_WhenEmisRespondsWith403()
        {
            // Arrange
            var nhsNumber = _fixture.Create<string>();
            var odsCode = _fixture.Create<string>();

            _emisClient.Setup(x => x.LinkageGet(nhsNumber, odsCode))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<LinkageDetailsResponse>(HttpStatusCode.Forbidden)));

            // Act
            var result = await _systemUnderTest.GetLinkageKey(nhsNumber, odsCode);

            // Assert
            _emisClient.Verify(x => x.LinkageGet(nhsNumber, odsCode));
            result.Should().BeAssignableTo<GetLinkageResult.LinkageKeyRevoked>();
        }

        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavailable_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            const string LinkageKey = "linkageKey";
            const string OdsCode = "odsCode";
            _emisClient.Setup(x => x.LinkageGet(LinkageKey, OdsCode))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetLinkageKey(LinkageKey, OdsCode);

            // Assert
            result.Should().BeAssignableTo<GetLinkageResult.SupplierSystemUnavailable>();
            _emisClient.Verify();
        }

        [TestMethod]
        public async Task Post_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            var linkageDetailsRequest = _fixture.Create<CreateLinkageRequest>();
            var linkageDetailsResponse = _fixture.Create<LinkageDetailsResponse>();

            _emisClient.Setup(x => x.LinkagePost(It.IsAny<LinkagePostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<LinkageDetailsResponse>(HttpStatusCode.OK)
                    {
                        Body = linkageDetailsResponse,
                    }));

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(linkageDetailsRequest);

            // Assert
            _emisClient.Verify(x => x.LinkagePost(It.Is<LinkagePostRequest>(request =>
                request.NhsNumber == linkageDetailsRequest.NhsNumber
                && request.OdsCode == linkageDetailsRequest.OdsCode
            )));
            _emisLinkageMapper.Verify(x => x.Map(linkageDetailsResponse));
            result.Should().BeAssignableTo<CreateLinkageResult.SuccessfullyRetrieved>();
            ((CreateLinkageResult.SuccessfullyRetrieved)result).Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Post_ReturnsNhsNumberNotFound_WhenEmisRespondsWith404()
        {
            // Arrange
            var linkageDetailsRequest = _fixture.Create<CreateLinkageRequest>();
            var linkageDetailsResponse = _fixture.Create<LinkageDetailsResponse>();

            _emisClient.Setup(x => x.LinkagePost(It.IsAny<LinkagePostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<LinkageDetailsResponse>(HttpStatusCode.NotFound)));

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(linkageDetailsRequest);

            // Assert
            _emisClient.Verify(x => x.LinkagePost(It.Is<LinkagePostRequest>(request =>
                request.NhsNumber == linkageDetailsRequest.NhsNumber
                && request.OdsCode == linkageDetailsRequest.OdsCode
            )));
            _emisLinkageMapper.Verify(x => x.Map(linkageDetailsResponse), Times.Never);
            result.Should().BeAssignableTo<CreateLinkageResult.NhsNumberNotFound>();
        }

        [TestMethod]
        public async Task Post_ReturnsLinkageKeyAlreadyExists_WhenEmisRespondsWith409()
        {
            // Arrange
            var linkageDetailsRequest = _fixture.Create<CreateLinkageRequest>();
            var linkageDetailsResponse = _fixture.Create<LinkageDetailsResponse>();

            _emisClient.Setup(x => x.LinkagePost(It.IsAny<LinkagePostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<LinkageDetailsResponse>(HttpStatusCode.Conflict)));

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(linkageDetailsRequest);

            // Assert
            _emisClient.Verify(x => x.LinkagePost(It.Is<LinkagePostRequest>(request =>
                request.NhsNumber == linkageDetailsRequest.NhsNumber
                && request.OdsCode == linkageDetailsRequest.OdsCode
            )));
            _emisLinkageMapper.Verify(x => x.Map(linkageDetailsResponse), Times.Never);
            result.Should().BeAssignableTo<CreateLinkageResult.LinkageKeyAlreadyExists>();
        }

        [TestMethod]
        public async Task Post_ReturnsSupplierSystemUnavailable_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            var linkageDetailsRequest = _fixture.Create<CreateLinkageRequest>();

            _emisClient.Setup(x => x.LinkagePost(It.IsAny<LinkagePostRequest>()))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(linkageDetailsRequest);

            // Assert
            result.Should().BeAssignableTo<CreateLinkageResult.SupplierSystemUnavailable>();
            _emisClient.Verify();
        }
    }
}