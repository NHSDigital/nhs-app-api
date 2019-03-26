using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NHSOnline.Backend.NominatedPharmacy.Soap;
using static NHSOnline.Backend.NominatedPharmacy.Soap.NominatedPharmacyTypes;
using FluentAssertions;
using NHSOnline.Backend.NominatedPharmacy.Clients;
using NHSOnline.Backend.NominatedPharmacy.Clients.Interfaces;
using NHSOnline.Backend.NominatedPharmacy.Clients.Models;
using NHSOnline.Backend.NominatedPharmacy.Models;

namespace NHSOnline.Backend.NominatedPharmacy.UnitTests
{
    [TestClass]
    public class NominatedPharmacyClientTests
    {
        private IFixture _fixture;
        private INominatedPharmacyClient _sut;
        private Mock<INominatedPharmacyPDSClient> _nominatedPharmacyPDSClientMock;
        private Mock<INominatedPharmacySubmitClient> _nominatedPharmacySubmitClientMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _nominatedPharmacyPDSClientMock = _fixture.Freeze<Mock<INominatedPharmacyPDSClient>>();
            _nominatedPharmacySubmitClientMock = _fixture.Freeze<Mock<INominatedPharmacySubmitClient>>();

            _sut = _fixture.Create<NominatedPharmacyClient>();
        }

        [TestMethod]
        public async Task NominatedPharmacyGet_ReturnsSuccessfully()
        {
            // Arrange
            var getNominatedPharmacyRequest = new QUPA_IN000008UK02();

            _nominatedPharmacyPDSClientMock
                .Setup(x => x.NominatedPharmacyGet(It.IsAny<QUPA_IN000008UK02>()))
                .Returns(Task.FromResult(
                    new NominatedPharmacyApiObjectResponse<QUPA_IN000009UK03_Response>(System.Net.HttpStatusCode.OK)
                    {
                        RawResponse = new NominatedPharmacyResponseEnvelope<QUPA_IN000009UK03_Response>(),
                    }
                ))
                .Verifiable();

            // Act
            var result = await _sut.NominatedPharmacyGet(getNominatedPharmacyRequest);

            // Assert
            _nominatedPharmacyPDSClientMock.Verify();
            result.Should()
                .BeOfType<NominatedPharmacyApiObjectResponse<QUPA_IN000009UK03_Response>>();
        }

        [TestMethod]
        public async Task NominatedPharmacyUpdate_ReturnsSuccessfully()
        {
            // Arrange
            var nominatedPharmacyUpdateRequest = _fixture.Create<NominatedPharmacyUpdateRequest>();

            _nominatedPharmacySubmitClientMock
                .Setup(x => x.UpdateNominatedPharmacy(It.IsAny<NominatedPharmacyUpdateRequest>()))
                .Returns(Task.FromResult(
                    new NominatedPharmacyApiObjectResponse<NominatedPharmacyUpdateResponse>(System.Net.HttpStatusCode.OK)
                    {
                        RawResponse = _fixture.Create<NominatedPharmacyResponseEnvelope<NominatedPharmacyUpdateResponse>>(),
                    }
                ))
                .Verifiable();

            // Act
            var result = await _sut.UpdateNominatedPharmacy(nominatedPharmacyUpdateRequest);

            // Assert
            _nominatedPharmacySubmitClientMock.Verify();
            result.Should()
                .BeOfType<NominatedPharmacyApiObjectResponse<NominatedPharmacyUpdateResponse>>();
        }
    }
}