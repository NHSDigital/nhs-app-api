using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord;

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
        private ILoggerFactory _loggerFactory;
        private ITppDemographicsMapper _tppDemographicsMapper;
        private ITppMyRecordMapper _tppMyRecordMapper;

        
        [TestInitialize]
        public void TestInitialize()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _options = new Mock<IOptions<ConfigurationSettings>>().Object;
            _serviceProvider = _mockServiceProvider.Object;
            _tppClient = new Mock<ITppClient>().Object;
            _systemUnderTest = new TppGpSystem(_serviceProvider);
            _loggerFactory = new Mock<ILoggerFactory>().Object;
            _tppDemographicsMapper = new Mock<ITppDemographicsMapper>().Object;
            _tppMyRecordMapper = new Mock<ITppMyRecordMapper>().Object;

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
        public void GetDemographicsService_WhenCalled_ReturnsTppDemographicsService()
        {
            var service = new TppDemographicsService(_loggerFactory, _tppClient, _tppDemographicsMapper);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(TppDemographicsService)))
                .Returns(service);

            _systemUnderTest.GetDemographicsService().Should().Be(service);
        }

        [TestMethod]
        public void GetPatientRecordService_WhenCalled_ReturnsTppPatientRecordService()
        {
            var service = new TppPatientRecordService(_loggerFactory, _tppClient, _tppMyRecordMapper);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(TppPatientRecordService)))
                .Returns(service);

            _systemUnderTest.GetPatientRecordService().Should().Be(service);
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
        public void GetPrescriptionService_WhenCalled_ReturnsTppPrescriptionService()
        {
            var mapper = Mock.Of<TppPrescriptionMapper>();
            var service = new TppPrescriptionService(_loggerFactory, _options, _tppClient, mapper);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(TppPrescriptionService)))
                .Returns(service);

            _systemUnderTest.GetPrescriptionService().Should().Be(service);
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