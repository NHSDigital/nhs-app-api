using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications.Azure;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications.Azure
{
    [TestClass]
    public class RegistrationDescriptionFactoryTest
    {
        private Fixture _fixture;
        private RegistrationDescriptionFactory _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());

            _systemUnderTest = _fixture.Create<RegistrationDescriptionFactory>();
        }

        [TestMethod]
        public void Create_WithValidAndroidRequest_ReturnsFcmRegistrationDescription()
        {
            // Arrange
            var nhsLoginId = _fixture.Create<string>();
            var request = _fixture.Build<RegisterDeviceRequest>()
                .With(x => x.DeviceType, DeviceType.Android).Create();

            var expectedResponse = new FcmRegistrationDescription(request.DevicePns)
            {
                Tags = new HashSet<string> { $"nhsLoginId:{nhsLoginId}" }
            };

            // Act
            var result = _systemUnderTest.Create(request, nhsLoginId);
            
            // Assert
            result.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Create_WithValidiOSRequest_ReturnsAppleRegistrationDescription()
        {
            // Arrange
            var nhsLoginId = _fixture.Create<string>();
            var request = _fixture.Build<RegisterDeviceRequest>()
                .With(x => x.DeviceType, DeviceType.Ios).Create();

            var expectedResponse = new AppleRegistrationDescription(request.DevicePns)
            {
                Tags = new HashSet<string> { $"nhsLoginId:{nhsLoginId}" }
            };

            // Act
            var result = _systemUnderTest.Create(request, nhsLoginId);
            
            // Assert
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Create_WithNoDeviceType_ThrowsAnException()
        {
            // Arrange
            var request = _fixture.Build<RegisterDeviceRequest>()
                .With(x => x.DeviceType, (DeviceType?)null).Create();

            // Act
            Action act = () => _systemUnderTest.Create(request, _fixture.Create<string>());

            // Assert
            act.Should()
                .Throw<ArgumentOutOfRangeException>()
                .And.ParamName.Should().Be("registerDeviceRequest");
        }
        
        [TestMethod]
        public void Create_WithNoNhsLoginId_ThrowsAnException()
        {
            // Arrange
            var request = _fixture.Build<RegisterDeviceRequest>()
                .Create();

            // Act
            Action act = () => _systemUnderTest.Create(request, null);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .And.ParamName.Should().Be("registerDeviceRequest");
        }
    }
}