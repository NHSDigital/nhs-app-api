using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Prescriptions
{
    [TestClass]
    public class VisionPrescriptionServiceTests
    {
        private VisionPrescriptionService _systemUnderTest;
        private Mock<IVisionClient> _visionClient;
        private Mock<IVisionPrescriptionMapper> _visionPrescriptionMapper;
        private VisionConfigurationSettings _settings;
        private VisionUserSession _visionUserSession;
        private IFixture _fixture;
        private string ApplicationProviderId = "ApplicationProviderId";
        private const string RequestUserName = "username";
        private const string certificatePassphrase = "CertificatePassphrase";
        private const string certificatePath = "CertificatePath";
        private const string visionSenderUserName = "visionuser";
        private const string visionSenderFullName = "visionuser";
        private const string visionSenderUserIdentity = "username";
        private const string visionSenderUserRole = "admin";
        private static readonly Uri ApiUrl = new Uri("http://vision_base_url/", UriKind.Absolute);
        private const int PrescriptionsMaxCoursesSoftLimit = 100;
        private const int CoursesMaxCoursesLimit = 100;
        private const int VisionAppointmentSlotsRequestCount = 100;
        private const string environment = "environment";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _visionUserSession = _fixture.Create<VisionUserSession>();
            _visionUserSession.IsRepeatPrescriptionsEnabled = true;
            _visionUserSession.AllowFreeTextPrescriptions = true;
            
            _visionClient = _fixture.Freeze<Mock<IVisionClient>>();
            _visionPrescriptionMapper = _fixture.Freeze<Mock<IVisionPrescriptionMapper>>();
            
            _settings = new VisionConfigurationSettings(ApplicationProviderId, ApiUrl, 
                certificatePath, certificatePassphrase, RequestUserName, visionSenderUserName, 
                visionSenderFullName, visionSenderUserIdentity, visionSenderUserRole, VisionAppointmentSlotsRequestCount, 
                CoursesMaxCoursesLimit, PrescriptionsMaxCoursesSoftLimit, environment);
                
            _fixture.Inject(_settings);
            _systemUnderTest = _fixture.Create<VisionPrescriptionService>();
        }

        [TestMethod]
        public async Task Get_ReturnsForbidden_WhenRepeatPrescriptionsIsDisabledInUserSession()
        {
            // Arrange
            _visionUserSession.IsRepeatPrescriptionsEnabled = false;

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_visionUserSession, null, null);

            // Assert
            _visionClient.VerifyNoOtherCalls();
            result.Should().BeOfType<GetPrescriptionsResult.Forbidden>();
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromVision()
        {
            // Arrange
            var prescriptionsHistoryResponse = new VisionResponseEnvelope<PrescriptionHistoryResponse>
            {
                Body = new VisionResponseBody<PrescriptionHistoryResponse>
                {
                    VisionResponse = new VisionResponse<PrescriptionHistoryResponse>
                    {
                        ServiceContent = new PrescriptionHistoryResponse
                        {
                            PrescriptionHistory = new PrescriptionHistory
                            {
                                Requests = new List<Request>(_fixture.CreateMany<Request>())
                            }
                        }
                    }
                }
            };
            
            _visionClient.Setup(x => x.GetHistoricPrescriptions(_visionUserSession, It.Is<PrescriptionRequest>(pr => string.Equals(pr.PatientId, _visionUserSession.PatientId, StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<PrescriptionHistoryResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = prescriptionsHistoryResponse,
                    }));

            var mappingResult = new PrescriptionListResponse
            {
                Prescriptions = new List<PrescriptionItem>(),
                Courses = new List<Course>(),
            };

            _visionPrescriptionMapper.Setup(x => x.Map(It.IsAny<PrescriptionHistory>())).Returns(mappingResult);

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_visionUserSession, null, null);

            // Assert
            _visionClient.Verify(x => x.GetHistoricPrescriptions(_visionUserSession, It.Is<PrescriptionRequest>(pr => string.Equals(pr.PatientId, _visionUserSession.PatientId, StringComparison.Ordinal))));
            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>();
            ((GetPrescriptionsResult.Success) result).Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Get_DoesNotReturnPrescriptionsWithStatusUnknown_WhenSuccessfulResponseFromVision()
        {
            // Arrange
            var prescriptionsHistoryResponse = new VisionResponseEnvelope<PrescriptionHistoryResponse>
            {
                Body = new VisionResponseBody<PrescriptionHistoryResponse>
                {
                    VisionResponse = new VisionResponse<PrescriptionHistoryResponse>
                    {
                        ServiceContent = new PrescriptionHistoryResponse
                        {
                            PrescriptionHistory = new PrescriptionHistory
                            {
                                Requests = new List<Request>(),
                            }
                        }
                    }
                }
            };

            _visionClient.Setup(x => x.GetHistoricPrescriptions(_visionUserSession, It.Is<PrescriptionRequest>(pr => string.Equals(pr.PatientId, _visionUserSession.PatientId, StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<PrescriptionHistoryResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = prescriptionsHistoryResponse,
                    }));

            var mappingResult = new PrescriptionListResponse
            {
                Prescriptions = new List<PrescriptionItem>
                {
                    new PrescriptionItem { Status = Backend.GpSystems.Prescriptions.Models.Status.Approved },
                    new PrescriptionItem { Status = Backend.GpSystems.Prescriptions.Models.Status.Rejected },
                    new PrescriptionItem { Status = Backend.GpSystems.Prescriptions.Models.Status.Requested },
                    new PrescriptionItem { Status = Backend.GpSystems.Prescriptions.Models.Status.Unknown },
                },
                Courses = new List<Course>(),
            };

            _visionPrescriptionMapper.Setup(x => x.Map(It.IsAny<PrescriptionHistory>())).Returns(mappingResult);

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_visionUserSession, null, null);

            // Assert
            _visionClient.Verify(x => x.GetHistoricPrescriptions(_visionUserSession, It.Is<PrescriptionRequest>(pr => string.Equals(pr.PatientId, _visionUserSession.PatientId, StringComparison.Ordinal))));
            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>();
            var successResult = (GetPrescriptionsResult.Success)result;
            successResult.Response.Should().NotBeNull();
            successResult.Response.Prescriptions.Count().Should().Be(3);
            successResult.Response.Prescriptions.ElementAt(0).Status.Should().Be(Backend.GpSystems.Prescriptions.Models.Status.Approved);
            successResult.Response.Prescriptions.ElementAt(1).Status.Should().Be(Backend.GpSystems.Prescriptions.Models.Status.Rejected);
            successResult.Response.Prescriptions.ElementAt(2).Status.Should().Be(Backend.GpSystems.Prescriptions.Models.Status.Requested);
        }

        [TestMethod]
        public async Task Get_PrescriptionsInResponseAreOrderedByDateRequestedDescending_WhenSuccessfulResponseFromVision()
        {
            // Arrange
            var prescriptionToday = new Request
            {
                Date = DateTime.Today,
            };

            var prescriptionYesterday = new Request
            {
                Date = DateTime.Today.AddDays(-1),
            };

            var prescriptionTomorrow = new Request
            {
                Date = DateTime.Today.AddDays(1),
            };

            var prescriptionsHistoryResponse = new VisionResponseEnvelope<PrescriptionHistoryResponse>
            {
                Body = new VisionResponseBody<PrescriptionHistoryResponse>
                {
                    VisionResponse = new VisionResponse<PrescriptionHistoryResponse>
                    {
                        ServiceContent = new PrescriptionHistoryResponse
                        {
                            PrescriptionHistory = new PrescriptionHistory
                            {
                                Requests = new List<Request>
                                {
                                    prescriptionToday,
                                    prescriptionYesterday,
                                    prescriptionTomorrow,
                                },
                            },
                        },
                    },
                },
            };

            _visionClient.Setup(x => x.GetHistoricPrescriptions(_visionUserSession, It.Is<PrescriptionRequest>(pr => string.Equals(pr.PatientId, _visionUserSession.PatientId, StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<PrescriptionHistoryResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = prescriptionsHistoryResponse,
                    }));

            var mappingResult = new PrescriptionListResponse
            {
                Prescriptions = new List<PrescriptionItem>(),
                Courses = new List<Course>(),
            };

            PrescriptionHistory capturedItemToMap = null;
            _visionPrescriptionMapper.Setup(x => x.Map(It.IsAny<PrescriptionHistory>())).Returns(mappingResult)
                .Callback<PrescriptionHistory>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_visionUserSession, null, null);

            // Assert
            _visionClient.Verify(x => x.GetHistoricPrescriptions(_visionUserSession, It.Is<PrescriptionRequest>(pr => string.Equals(pr.PatientId, _visionUserSession.PatientId, StringComparison.Ordinal))));
            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>();
            ((GetPrescriptionsResult.Success) result).Response.Should().NotBeNull();

            var getPrescriptionsResult = (GetPrescriptionsResult.Success) result;
            getPrescriptionsResult.Response.Should().Be(mappingResult);

            capturedItemToMap.Requests.Should().HaveCount(3);

            capturedItemToMap.Requests.ElementAt(0).Should().Be(prescriptionTomorrow);
            capturedItemToMap.Requests.ElementAt(1).Should().Be(prescriptionToday);
            capturedItemToMap.Requests.ElementAt(2).Should().Be(prescriptionYesterday);
        }

        [TestMethod]
        public async Task Get_PrescriptionsInResponseAreFilteredByDate_WhenSuccessfulResponseFromVision()
        {
            // Arrange
            var fromDateOneWeekAgo = DateTime.Today.AddDays(-7);
            var toDateToday = DateTime.Today;

            var prescriptionTomorrow = new Request
            {
                Date = DateTime.Today.AddDays(1),
            };

            var prescriptionToday = new Request
            {
                Date = DateTime.Today,
            };

            var prescriptionYesterday = new Request
            {
                Date = DateTime.Today.AddDays(-1),
            };

            var prescriptionOneWeekAgo = new Request
            {
                Date = DateTime.Today.AddDays(-7),
            };

            var prescriptionOneWeekOneDayAgo = new Request
            {
                Date = DateTime.Today.AddDays(-8),
            };


            var prescriptionsHistoryResponse = new VisionResponseEnvelope<PrescriptionHistoryResponse>
            {
                Body = new VisionResponseBody<PrescriptionHistoryResponse>
                {
                    VisionResponse = new VisionResponse<PrescriptionHistoryResponse>
                    {
                        ServiceContent = new PrescriptionHistoryResponse
                        {
                            PrescriptionHistory = new PrescriptionHistory
                            {
                                Requests = new List<Request>
                                {
                                    prescriptionTomorrow,
                                    prescriptionToday,
                                    prescriptionYesterday,
                                    prescriptionOneWeekAgo,
                                    prescriptionOneWeekOneDayAgo,
                                }
                            }
                        }
                    }
                }
            };

            _visionClient.Setup(x => x.GetHistoricPrescriptions(_visionUserSession, It.Is<PrescriptionRequest>(pr => string.Equals(pr.PatientId, _visionUserSession.PatientId, StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<PrescriptionHistoryResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = prescriptionsHistoryResponse,
                    }));

            var mappingResult = new PrescriptionListResponse
            {
                Prescriptions = new List<PrescriptionItem>(),
                Courses = new List<Course>(),
            };

            PrescriptionHistory capturedItemToMap = null;
            _visionPrescriptionMapper.Setup(x => x.Map(It.IsAny<PrescriptionHistory>())).Returns(mappingResult)
                .Callback<PrescriptionHistory>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_visionUserSession, fromDateOneWeekAgo, toDateToday);

            // Assert
            _visionClient.Verify(x => x.GetHistoricPrescriptions(_visionUserSession, It.Is<PrescriptionRequest>(pr => string.Equals(pr.PatientId, _visionUserSession.PatientId, StringComparison.Ordinal))));
            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>();
            ((GetPrescriptionsResult.Success)result).Response.Should().NotBeNull();

            var getPrescriptionsResult = (GetPrescriptionsResult.Success)result;
            getPrescriptionsResult.Response.Should().Be(mappingResult);

            capturedItemToMap.Requests.Should().HaveCount(3);

            capturedItemToMap.Requests.ElementAt(0).Should().Be(prescriptionToday);
            capturedItemToMap.Requests.ElementAt(1).Should().Be(prescriptionYesterday);
            capturedItemToMap.Requests.ElementAt(2).Should().Be(prescriptionOneWeekAgo);
        }
        
        [DataTestMethod]
        [DataRow(PrescriptionsMaxCoursesSoftLimit + 1, PrescriptionsMaxCoursesSoftLimit)]
        [DataRow(PrescriptionsMaxCoursesSoftLimit, PrescriptionsMaxCoursesSoftLimit)]
        [DataRow(PrescriptionsMaxCoursesSoftLimit - 1, PrescriptionsMaxCoursesSoftLimit - 1)]
        public async Task Get_PrescriptionsInResponseAreLimitedToMax_WhenSuccessfulResponseFromVision(
            int numberOfCoursesToCreate, int expectedNumberOfPrescriptions)
        {
            // Arrange
            var prescriptionHistory = new PrescriptionHistory();

            for (int i = 0; i < numberOfCoursesToCreate; i++)
            {
                prescriptionHistory.Requests.Add(new Request
                {
                    Repeats = new List<GetPrescriptionRepeat>
                    {
                        new GetPrescriptionRepeat(),
                    },
                });
            }

            var prescriptionsHistoryResponse = new VisionResponseEnvelope<PrescriptionHistoryResponse>
            {
                Body = new VisionResponseBody<PrescriptionHistoryResponse>
                {
                    VisionResponse = new VisionResponse<PrescriptionHistoryResponse>
                    {
                        ServiceContent = new PrescriptionHistoryResponse
                        {
                            PrescriptionHistory = prescriptionHistory,
                        },
                    },
                },
            };

            _visionClient.Setup(x => x.GetHistoricPrescriptions(_visionUserSession, It.Is<PrescriptionRequest>(pr => string.Equals(pr.PatientId, _visionUserSession.PatientId, StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<PrescriptionHistoryResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = prescriptionsHistoryResponse,
                    }));

            var mappingResult = new PrescriptionListResponse
            {
                Prescriptions = new List<PrescriptionItem>(),
                Courses = new List<Course>(),
            };

            PrescriptionHistory capturedItemToMap = null;
            _visionPrescriptionMapper.Setup(x => x.Map(It.IsAny<PrescriptionHistory>())).Returns(mappingResult)
                .Callback<PrescriptionHistory>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_visionUserSession, null, null);

            // Assert
            _visionClient.Verify(x => x.GetHistoricPrescriptions(_visionUserSession, It.Is<PrescriptionRequest>(pr => string.Equals(pr.PatientId, _visionUserSession.PatientId, StringComparison.Ordinal))));
            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>();
            ((GetPrescriptionsResult.Success)result).Response.Should().NotBeNull();

            var getPrescriptionsResult = (GetPrescriptionsResult.Success)result;
            getPrescriptionsResult.Response.Should().Be(mappingResult);

            capturedItemToMap.Requests.Should().HaveCount(expectedNumberOfPrescriptions);
        }
        
        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavilable_WhenHttpStatusCodeIndicatesErrorFromVision()
        {
            // Arrange
            _visionClient.Setup(x => x.GetHistoricPrescriptions(_visionUserSession, 
                    It.Is<PrescriptionRequest>(pr => string.Equals(pr.PatientId,_visionUserSession.PatientId,StringComparison.Ordinal))))
               .Returns(Task.FromResult(
                   new VisionPFSClient.VisionApiObjectResponse<PrescriptionHistoryResponse>(HttpStatusCode.InternalServerError)));
            
            // Act
            var result = await _systemUnderTest.GetPrescriptions(_visionUserSession, null, null);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.BadGateway>();
        }

        [TestMethod]
        public async Task Get_ReturnsBadGateway_WhenHttpExceptionOccursCallingVision()
        {
            // Arrange
            _visionClient.Setup(x => x.GetHistoricPrescriptions(_visionUserSession, 
                    It.Is<PrescriptionRequest>(pr => string.Equals(pr.PatientId, _visionUserSession.PatientId, StringComparison.Ordinal))))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_visionUserSession, null, null);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.BadGateway>();
            _visionClient.Verify();
        }

        [TestMethod]
        public async Task OrderPrescription_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromVision()
        {
            // Arrange
            var orderPrescriptionResponse = new VisionResponseEnvelope<OrderNewPrescriptionResponse>
            {
                Body = new VisionResponseBody<OrderNewPrescriptionResponse>
                {
                    VisionResponse = new VisionResponse<OrderNewPrescriptionResponse>
                    {
                        ServiceContent = new OrderNewPrescriptionResponse
                        {
                            Result = OrderNewPrescriptionResponse.OkResponseText,
                        },
                    },
                },
            };

            var request = new RepeatPrescriptionRequest
            {
                CourseIds = new[] { "2", "5", "7" },
                SpecialRequest = "quick please",
            };

            OrderNewPrescriptionRequest capturedRequest = null;

            _visionClient.Setup(x => x.OrderNewPrescription(
                _visionUserSession,
                It.IsAny<OrderNewPrescriptionRequest>()))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<OrderNewPrescriptionResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = orderPrescriptionResponse,
                    }))
                    .Callback<VisionUserSession, OrderNewPrescriptionRequest>((visionUserSession, orderNewPrescriptionRequest) => capturedRequest = orderNewPrescriptionRequest);
            
            // Act
            var result = await _systemUnderTest.OrderPrescription(_visionUserSession, request);

            // Assert
            var expectedRequest = new OrderNewPrescriptionRequest
            {
                Repeats = request.CourseIds.Select(x => new NewPrescriptionRepeat { Id = x }).ToList(),
                Message = request.SpecialRequest,
                PatientId = _visionUserSession.PatientId,
            };

            capturedRequest.Should().BeEquivalentTo(expectedRequest);
            _visionClient.Verify(x => x.OrderNewPrescription(_visionUserSession, capturedRequest));
            result.Should().BeAssignableTo<OrderPrescriptionResult.Success>();
            ((OrderPrescriptionResult.Success)result).Should().NotBeNull();
        }

        [TestMethod]
        public async Task OrderPrescription_ReturnsForbidden_WhenRepeatPrescriptionsIsDisabledInUserSession()
        {
            // Arrange
            _visionUserSession.IsRepeatPrescriptionsEnabled = false;

            var request = new RepeatPrescriptionRequest
            {
                CourseIds = new[] { "2", "5", "7" },
                SpecialRequest = "quick please",
            };

            // Act
            var result = await _systemUnderTest.OrderPrescription(_visionUserSession, request);

            // Assert
            _visionClient.VerifyNoOtherCalls();
            result.Should().BeOfType<OrderPrescriptionResult.Forbidden>();
        }

        [TestMethod]
        public async Task OrderPrescription_ReturnsBadRequest_WhenRepeatPrescriptionsIsDisabledInUserSession()
        {
            // Arrange
            _visionUserSession.AllowFreeTextPrescriptions = false;

            var request = new RepeatPrescriptionRequest
            {
                CourseIds = new[] { "1" },
                SpecialRequest = "special request text not allowed",
            };

            // Act
            var result = await _systemUnderTest.OrderPrescription(_visionUserSession, request);

            // Assert
            _visionClient.VerifyNoOtherCalls();
            result.Should().BeOfType<OrderPrescriptionResult.BadRequest>();
        }

        [TestMethod]
        public async Task OrderPrescription_AllowsPrescriptionToBeSubmittedWithNullSpecialRequestText_WhenSpecialRequestTextIsDisabledInUserSession()
        {
            // Arrange
            _visionUserSession.AllowFreeTextPrescriptions = false;

            var orderPrescriptionResponse = new VisionResponseEnvelope<OrderNewPrescriptionResponse>
            {
                Body = new VisionResponseBody<OrderNewPrescriptionResponse>
                {
                    VisionResponse = new VisionResponse<OrderNewPrescriptionResponse>
                    {
                        ServiceContent = new OrderNewPrescriptionResponse
                        {
                            Result = OrderNewPrescriptionResponse.OkResponseText,
                        },
                    },
                },
            };

            var request = new RepeatPrescriptionRequest
            {
                CourseIds = new[] { "1" },
                SpecialRequest = null,
            };

            // Act
            OrderNewPrescriptionRequest capturedRequest = null;

            _visionClient.Setup(x => x.OrderNewPrescription(
                _visionUserSession,
                It.IsAny<OrderNewPrescriptionRequest>()))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<OrderNewPrescriptionResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = orderPrescriptionResponse,
                    }))
                    .Callback<VisionUserSession, OrderNewPrescriptionRequest>((visionUserSession, orderNewPrescriptionRequest) => capturedRequest = orderNewPrescriptionRequest);

            // Act
            var result = await _systemUnderTest.OrderPrescription(_visionUserSession, request);

            // Assert
            var expectedRequest = new OrderNewPrescriptionRequest
            {
                Repeats = request.CourseIds.Select(x => new NewPrescriptionRepeat { Id = x }).ToList(),
                Message = request.SpecialRequest,
                PatientId = _visionUserSession.PatientId,
            };

            capturedRequest.Should().BeEquivalentTo(expectedRequest);
            _visionClient.Verify(x => x.OrderNewPrescription(_visionUserSession, capturedRequest));
            result.Should().BeAssignableTo<OrderPrescriptionResult.Success>();
            ((OrderPrescriptionResult.Success)result).Should().NotBeNull();
        }

        [TestMethod]
        public async Task OrderPrescription_ReturnsSupplierSystemUnavilable_WhenHttpStatusCodeIndicatesErrorFromVision()
        {
            // Arrange
            _visionClient.Setup(x => x.OrderNewPrescription(_visionUserSession, It.IsAny<OrderNewPrescriptionRequest>()))
               .Returns(Task.FromResult(
                   new VisionPFSClient.VisionApiObjectResponse<OrderNewPrescriptionResponse>(HttpStatusCode.InternalServerError)));

            var request = _fixture.Create<RepeatPrescriptionRequest>();

            // Act
            var result = await _systemUnderTest.OrderPrescription(_visionUserSession, request);

            // Assert
            result.Should().BeAssignableTo<OrderPrescriptionResult.BadGateway>();
        }

        [TestMethod]
        public async Task OrderPrescription_ReturnsSupplierSystemUnavilable_WhenServiceContentIsNotEqualToOk()
        {
            // Arrange
            var orderPrescriptionResponse = new VisionResponseEnvelope<OrderNewPrescriptionResponse>
            {
                Body = new VisionResponseBody<OrderNewPrescriptionResponse>
                {
                    VisionResponse = new VisionResponse<OrderNewPrescriptionResponse>
                    {
                        ServiceContent = new OrderNewPrescriptionResponse
                        {
                            Result = "text not equal to ok",
                        },
                    },
                },
            };

            _visionClient.Setup(x => x.OrderNewPrescription(_visionUserSession, It.IsAny<OrderNewPrescriptionRequest>()))
               .Returns(Task.FromResult(
                   new VisionPFSClient.VisionApiObjectResponse<OrderNewPrescriptionResponse>(HttpStatusCode.OK)
                   {
                       RawResponse = orderPrescriptionResponse,
                   }));

            var request = _fixture.Create<RepeatPrescriptionRequest>();

            // Act
            var result = await _systemUnderTest.OrderPrescription(_visionUserSession, request);

            // Assert
            result.Should().BeAssignableTo<OrderPrescriptionResult.BadGateway>();
        }

        [TestMethod]
        public async Task OrderPrescription_ReturnsBadGateway_WhenHttpExceptionOccursCallingVision()
        {
            // Arrange
            _visionClient.Setup(x => x.OrderNewPrescription(_visionUserSession, It.IsAny<OrderNewPrescriptionRequest>())).Throws<HttpRequestException>()
                .Verifiable();

            var request = _fixture.Create<RepeatPrescriptionRequest>();

            // Act
            var result = await _systemUnderTest.OrderPrescription(_visionUserSession, request);

            // Assert
            result.Should().BeAssignableTo<OrderPrescriptionResult.BadGateway>();
            _visionClient.Verify();
        }
    }
}
