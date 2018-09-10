using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.Worker.Support.Temporal;

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
        
        private ILogger<TppCourseService> _tppCourseLogger;
        private ILogger<TppPrescriptionService> _tppPrescriptionLogger;
        
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
            
            _tppCourseLogger = Mock.Of<ILogger<TppCourseService>>();
            _tppPrescriptionLogger = Mock.Of<ILogger<TppPrescriptionService>>();
        }

        [TestMethod]
        public void Supplier_ReturnsTppSupplier()
        {
            _systemUnderTest.Supplier.Should().Be(Supplier.Tpp);
        }

        [TestMethod]
        public void GetAppointmentsService_WhenCalled_ReturnsTppAppointmentService()
        {
            var dateTimeOffsetProvider = Mock.Of<IDateTimeOffsetProvider>();
            var builder = Mock.Of<IAppointmentsResultBuilder>();
            var service = new TppAppointmentsService(
                new TppAppointmentsRetrievalService(Mock.Of<ILogger<TppAppointmentsRetrievalService>>(), _tppClient, builder),
                new TppAppointmentsBookingService(Mock.Of<ILogger<TppAppointmentsBookingService>>(), _tppClient, dateTimeOffsetProvider),
                new TppAppointmentsCancellationService(Mock.Of<ILogger<TppAppointmentsCancellationService>>(), _tppClient));

            _mockServiceProvider.Setup(x => x.GetService(typeof(TppAppointmentsService)))
                .Returns(service);

            _systemUnderTest.GetAppointmentsService().Should().Be(service);
        }
        
        [TestMethod]
        public void GetAppointmentSlotsService_WhenCalled_ReturnsTppAppointmentSlotsService()
        {
            var builder = Mock.Of<IAppointmentSlotResultBuilder>();
            var logger = Mock.Of<ILogger<TppAppointmentSlotsService>>();
            var service = new TppAppointmentSlotsService(_tppClient, logger, builder);

            _mockServiceProvider.Setup(x => x.GetService(typeof(TppAppointmentSlotsService)))
                .Returns(service);

            _systemUnderTest.GetAppointmentSlotsService().Should().Be(service);
        }
        
        [TestMethod]
        public void GetCourseService_WhenCalled_ReturnsTppCourseService()
        {
            var logger = Mock.Of<ILogger<TppCourseMapper>>();
            var mapper = new TppCourseMapper(logger);
            
            var service = new TppCourseService(_tppCourseLogger, _options, _tppClient, mapper);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(TppCourseService)))
                .Returns(service);

            _systemUnderTest.GetCourseService().Should().Be(service);
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
            var service = new TppPatientRecordService(_loggerFactory, _tppClient, 
                _tppMyRecordMapper);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(TppPatientRecordService)))
                .Returns(service);

            _systemUnderTest.GetPatientRecordService().Should().Be(service);
        }

        [TestMethod]
        public void GetIm1ConnectionService_WhenCalled_ReturnsTppIm1ConnectionService()
        {
            var service = new TppIm1ConnectionService(_tppClient, _loggerFactory.CreateLogger<TppIm1ConnectionService>());
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(TppIm1ConnectionService)))
                .Returns(service);

            _systemUnderTest.GetIm1ConnectionService().Should().Be(service);
        }
        
        [TestMethod]
        public void GetPrescriptionService_WhenCalled_ReturnsTppPrescriptionService()
        {
            
            var logger = Mock.Of<ILogger<TppPrescriptionMapper>>();
            var mapper = new TppPrescriptionMapper(logger);
            
            var service = new TppPrescriptionService(_tppPrescriptionLogger, _options, _tppClient, mapper);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(TppPrescriptionService)))
                .Returns(service);

            _systemUnderTest.GetPrescriptionService().Should().Be(service);
        }
        
        [TestMethod]
        public void GetSessionService_WhenCalled_ReturnsTppSessionService()
        {
            var logger = Mock.Of<ILogger<TppSessionService>>();
            var service = new TppSessionService(_tppClient,logger);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(TppSessionService)))
                .Returns(service);

            _systemUnderTest.GetSessionService().Should().Be(service);
        }
        
        [TestMethod]
        public void GetTokenValidationService_WhenCalled_ReturnsTppTokenValidationService()
        {
            var logger = Mock.Of<ILogger<TppTokenValidationService>>();
            var service = new TppTokenValidationService(logger);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(TppTokenValidationService)))
                .Returns(service);

            _systemUnderTest.GetTokenValidationService().Should().Be(service);
        }
    }
}
