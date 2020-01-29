using System;
using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RichardSzalay.MockHttp;
using NHSOnline.Backend.NominatedPharmacy.Clients;

namespace NHSOnline.Backend.NominatedPharmacy.UnitTests.Clients
{
    [TestClass]
    public sealed class NominatedPharmacyHttpClientTests : IDisposable
    {
        private NominatedPharmacyHttpClient _sut;
        private Mock<INominatedPharmacyConfigurationSettings> _configMock;
        private MockHttpMessageHandler _mockHttpHandler;
        private IFixture _fixture;

        private const string BaseUrl = "http://test";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _configMock = _fixture.Freeze<Mock<INominatedPharmacyConfigurationSettings>>();
            _mockHttpHandler = new MockHttpMessageHandler();
        }

        [TestMethod]
        public void Constructor_AddsEventHandlerToListenForSettingUpdates_WhenBaseUrlIsNull()
        {
            // Arrange
            _configMock.SetupGet(x => x.BaseUrl).Returns((Uri)null);
            _configMock.SetupAdd(x => x.SettingsUpdated += (sender, args) => {});

            // Act
            _sut = new NominatedPharmacyHttpClient(new HttpClient(_mockHttpHandler), _configMock.Object);

            // Assert
            _sut.Client.BaseAddress.Should().BeNull();
            _configMock.VerifyAdd(x => x.SettingsUpdated += It.IsAny<EventHandler<NominatedPharmacyConfigurationUpdatedEventArgs>>(), Times.Once);
        }

        [TestMethod]
        public void Constructor_DoesNotAddEventHandlerToListenForSettingUpdates_WhenBaseUrlIsPopulated()
        {
            // Arrange
            _configMock.SetupGet(x => x.BaseUrl).Returns(new Uri(BaseUrl));
            _configMock.SetupAdd(x => x.SettingsUpdated += (sender, args) => {});

            // Act
            _sut = new NominatedPharmacyHttpClient(new HttpClient(_mockHttpHandler), _configMock.Object);

            // Assert
            _sut.Client.BaseAddress.Should().Be(BaseUrl);
            _configMock.VerifyAdd(x => x.SettingsUpdated += It.IsAny<EventHandler<NominatedPharmacyConfigurationUpdatedEventArgs>>(), Times.Never);
        }

        [TestMethod]
        public void ConfigOnSettingsUpdated_RemovesEventHandler_WhenSettingsUpdatedCalled()
        {
            // Arrange
            _configMock.SetupGet(x => x.BaseUrl).Returns((Uri)null);
            _configMock.SetupAdd(x => x.SettingsUpdated += (sender, args) => {});

            _sut = new NominatedPharmacyHttpClient(new HttpClient(_mockHttpHandler), _configMock.Object);
            _configMock.SetupGet(x => x.BaseUrl).Returns(new Uri(BaseUrl));

            // Act
            _configMock.Raise(x => x.SettingsUpdated += null, new NominatedPharmacyConfigurationUpdatedEventArgs
            {
                Config = _configMock.Object,
            });

            // Assert
            _sut.Client.BaseAddress.Should().Be(BaseUrl);
            _configMock.VerifyRemove(x => x.SettingsUpdated -= It.IsAny<EventHandler<NominatedPharmacyConfigurationUpdatedEventArgs>>(), Times.Once);
        }

        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }
    }
}