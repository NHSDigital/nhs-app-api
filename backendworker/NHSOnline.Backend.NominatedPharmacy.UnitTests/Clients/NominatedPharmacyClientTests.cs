using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.NominatedPharmacy.Soap;
using NHSOnline.Backend.NominatedPharmacy.Clients;
using NHSOnline.Backend.NominatedPharmacy.Clients.Interfaces;
using NHSOnline.Backend.NominatedPharmacy.Clients.Models;
using NHSOnline.Backend.NominatedPharmacy.Models;
using static NHSOnline.Backend.NominatedPharmacy.Soap.GetNominatedPharmacyTypes;

namespace NHSOnline.Backend.NominatedPharmacy.UnitTests.Clients
{
    [TestClass]
    public class NominatedPharmacyClientTests
    {
        private IFixture _fixture;
        private INominatedPharmacyClient _systemUnderTest;
        private Mock<INominatedPharmacyPDSClient> _nominatedPharmacyPDSClientMock;
        private Mock<INominatedPharmacySubmitClient> _nominatedPharmacySubmitClientMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _nominatedPharmacyPDSClientMock = _fixture.Freeze<Mock<INominatedPharmacyPDSClient>>();
            _nominatedPharmacySubmitClientMock = _fixture.Freeze<Mock<INominatedPharmacySubmitClient>>();

            _systemUnderTest = _fixture.Create<NominatedPharmacyClient>();
        }

        [TestMethod]
        public async Task NominatedPharmacyGet_ReturnsSuccessfully()
        {
            // Arrange
            var getNominatedPharmacyRequest = new QUPAIN000008UK02();

            _nominatedPharmacyPDSClientMock
                .Setup(x => x.NominatedPharmacyGet(It.IsAny<QUPAIN000008UK02>()))
                .Returns(Task.FromResult(
                    new NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response>(System.Net.HttpStatusCode.OK)
                    {
                        RawResponse = new NominatedPharmacyResponseEnvelope<QUPAIN000009UK03Response>(),
                    }
                ))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.NominatedPharmacyGet(getNominatedPharmacyRequest);

            // Assert
            _nominatedPharmacyPDSClientMock.Verify();
            result.Should()
                .BeOfType<NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response>>();
        }

        [TestMethod]
        public async Task NominatedPharmacyUpdate_ReturnsSuccessfully()
        {
            // Arrange
            var nominatedPharmacyUpdateRequest = _fixture.Create<NominatedPharmacyUpdateRequest>();

            _nominatedPharmacySubmitClientMock
                .Setup(x => x.UpdateNominatedPharmacy(It.IsAny<NominatedPharmacyUpdateRequest>()))
                .Returns(Task.FromResult(
                    new UpdateNominatedPharmacyApiObjectResponse(System.Net.HttpStatusCode.OK)
                    {
                        Response = _fixture.Create<UpdateNominatedPharmacyResponseEnvelope>(),
                    }
                ))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(nominatedPharmacyUpdateRequest);

            // Assert
            _nominatedPharmacySubmitClientMock.Verify();
            result.Should()
                .BeOfType<UpdateNominatedPharmacyApiObjectResponse>();
        }
    }
}