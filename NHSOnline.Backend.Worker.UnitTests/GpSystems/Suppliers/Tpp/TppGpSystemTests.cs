using System;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp
{
    [TestClass]
    public class TppGpSystemTests
    {
        private IOptions<ConfigurationSettings> _options;
        private IServiceProvider _serviceProvider;
        private ITppClient _tppClient;
        private Mock<IServiceProvider> _mockServiceProvider;
        private TppGpSystem _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _options = new Mock<IOptions<ConfigurationSettings>>().Object;
            _serviceProvider = _mockServiceProvider.Object;
            _tppClient = new Mock<ITppClient>().Object;
            _systemUnderTest = new TppGpSystem(_serviceProvider);    
        }

        [TestMethod]
        public void Supplier_ReturnsTppSupplier()
        {
            _systemUnderTest.Supplier.Should().Be(SupplierEnum.Tpp);
        }

        [TestMethod]
        public void GetAppointmentsService_WhenCalled_ThrowsNotImplementedException()
        {
            (new Action(() => _systemUnderTest.GetAppointmentsService()))
                .Should()
                .Throw<NotImplementedException>();
        }
        
        [TestMethod]
        public void GetAppointmentSlotsService_WhenCalled_ThrowsNotImplementedException()
        {
            (new Action(() => _systemUnderTest.GetAppointmentSlotsService()))
                .Should()
                .Throw<NotImplementedException>();
        }
        
        [TestMethod]
        public void GetCourseService_WhenCalled_ThrowsNotImplementedException()
        {
            (new Action(() => _systemUnderTest.GetCourseService()))
                .Should()
                .Throw<NotImplementedException>();
        }
        
        [TestMethod]
        public void GetDemographicsService_WhenCalled_ThrowsNotImplementedException()
        {
            (new Action(() => _systemUnderTest.GetDemographicsService()))
                .Should()
                .Throw<NotImplementedException>();
        }

        [TestMethod]
        public void GetPatientRecordService_WhenCalled_ThrowsNotImplementedException()
        {
            (new Action(() => _systemUnderTest.GetPatientRecordService()))
                .Should()
                .Throw<NotImplementedException>();
        }

        [TestMethod]
        public void GetIm1ConnectionService_WhenCalled_ReturnsTppIm1ConnectionService()
        {
            var service = new TppIm1ConnectionService(_tppClient);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(TppIm1ConnectionService)))
                .Returns(service);

            _systemUnderTest.GetIm1ConnectionService().Should().Be(service);
        }
        
        [TestMethod]
        public void GetPrescriptionService_WhenCalled_ThrowsNotImplementedException()
        {
            (new Action(() => _systemUnderTest.GetPrescriptionService()))
                .Should()
                .Throw<NotImplementedException>();
        }
        
        [TestMethod]
        public void GetSessionService_WhenCalled_ReturnsTppSessionService()
        {
            var service = new TppSessionService(_tppClient, _options);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(TppSessionService)))
                .Returns(service);

            _systemUnderTest.GetSessionService().Should().Be(service);
        }
        
        [TestMethod]
        public void GetTokenValidationService_WhenCalled_ReturnsTppTokenValidationService()
        {
            var service = new TppTokenValidationService();
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(TppTokenValidationService)))
                .Returns(service);

            _systemUnderTest.GetTokenValidationService().Should().Be(service);
        }
    }
}