using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
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
            //Arrange
            var request = _fixture.Build<NotificationRegistrationRequest>()
                .With(x => x.DeviceType, DeviceType.Android).Create();

            var expectedResponse = new FcmRegistrationDescription(request.DevicePns)
            {
                Tags = new HashSet<string> { $"nhsLoginId:{request.NhsLoginId}" }
            };

            //Act
            var result = _systemUnderTest.Create(request);
            
            //Assert
            result.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Create_WithValidiOSRequest_ReturnsAppleRegistrationDescription()
        {
            //Arrange
            var request = _fixture.Build<NotificationRegistrationRequest>()
                .With(x => x.DeviceType, DeviceType.Ios).Create();

            var expectedResponse = new AppleRegistrationDescription(request.DevicePns)
            {
                Tags = new HashSet<string> { $"nhsLoginId:{request.NhsLoginId}" }
            };

            //Act
            var result = _systemUnderTest.Create(request);
            
            //Assert
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Create_WithNoDeviceType_ThrowsAnException()
        {
            //Arrange
            var request = _fixture.Build<NotificationRegistrationRequest>()
                .With(x => x.DeviceType, (DeviceType?)null).Create();

            //Act
            Action act = () => _systemUnderTest.Create(request);

            //Assert
            act.Should()
                .Throw<ArgumentOutOfRangeException>()
                .And.ParamName.Should().Be("notificationRegistrationRequest");
        }
        
        [TestMethod]
        public void Create_WithNoNhsLoginId_ThrowsAnException()
        {
            //Arrange
            var request = _fixture.Build<NotificationRegistrationRequest>()
                .Without(x => x.NhsLoginId)
                .Create();

            //Act
            Action act = () => _systemUnderTest.Create(request);

            //Assert
            act.Should()
                .Throw<ArgumentException>()
                .And.ParamName.Should().Be("notificationRegistrationRequest");
        }
    }
}