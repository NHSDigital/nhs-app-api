using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Prescriptions;
using NHSOnline.Backend.Support;
using Course = NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions.Course;
using MappedCourse = NHSOnline.Backend.GpSystems.Prescriptions.Models.Course;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Prescriptions
{
    [TestClass]
    public class MicrotestPrescriptionMapperTests
    {
        private IFixture _fixture;
        private IMicrotestPrescriptionMapper _mapper;
        private ILogger<MicrotestPrescriptionMapper> _logger;
        private Mock<IGpSystemFactory> _gpSystemFactoryMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _logger = Mock.Of<ILogger<MicrotestPrescriptionMapper>>();

            _gpSystemFactoryMock = new Mock<IGpSystemFactory>();
            _gpSystemFactoryMock
                .Setup(f => f.CreateGpSystem(Supplier.Microtest))
                .Returns(_fixture.Create<MicrotestGpSystem>());

            _mapper = new MicrotestPrescriptionMapper(_logger, _gpSystemFactoryMock.Object);
        }

        [TestMethod]
        public void MapPrescriptionHistoryGetResponseToPrescriptionListResponse_WhenPassingNull_ThrowsNullReferenceException()
        {
            // Act
            Action act = () => _mapper.Map((PrescriptionHistoryGetResponse)null);

            // Assert
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("prescriptionHistoryGetResponse");
        }

        [TestMethod]
        public void MapPrescriptionHistoryGetResponseToPrescriptionListResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new PrescriptionHistoryGetResponse();

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Prescriptions.Should().BeEmpty();
            result.Courses.Should().BeEmpty();
        }

        [TestMethod]
        public void MapPrescriptionHistoryGetResponseToPrescriptionListResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var item = new PrescriptionHistoryGetResponse
            {
                Courses = new List<PrescriptionCourse>
                {
                    new PrescriptionCourse
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "paracetamol",
                        Dosage = "take one each morning",
                        Quantity = "1 tablet",
                        Type = PrescriptionType.Repeat,
                        Status = PrescriptionStatus.Requested,
                        OrderDate = _fixture.Create<DateTimeOffset>(),
                        Reason = string.Empty,
                    },
                    new PrescriptionCourse
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "amoxicillin",
                        Dosage = "take two each afternoon",
                        Quantity = "2 tablet",
                        Type = PrescriptionType.Repeat,
                        Status = PrescriptionStatus.Requested,
                        OrderDate = _fixture.Create<DateTimeOffset>(),
                        Reason = string.Empty,
                    },
                    new PrescriptionCourse
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "epogen",
                        Dosage = "take one each afternoon",
                        Quantity = "1 injection",
                        Type = PrescriptionType.Repeat,
                        Status = PrescriptionStatus.Confirmed,
                        OrderDate = _fixture.Create<DateTimeOffset>(),
                        Reason = string.Empty,
                    },
                }
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            var expectedResult = new PrescriptionListResponse
            {
                Prescriptions = new List<PrescriptionItem>
                {
                    new PrescriptionItem
                    {
                        OrderDate = item.Courses.ElementAt(0).OrderDate,
                        Status = Status.Requested,
                        Courses = new List<CourseEntry>
                        {
                            new CourseEntry
                            {
                                CourseId = item.Courses.ElementAt(0).Id,
                            },
                        }
                    },
                    new PrescriptionItem
                    {
                        OrderDate = item.Courses.ElementAt(1).OrderDate,
                        Status = Status.Requested,
                        Courses = new List<CourseEntry>
                        {
                            new CourseEntry
                            {
                                CourseId = item.Courses.ElementAt(1).Id,
                            },
                        }
                    },
                    new PrescriptionItem
                    {
                        OrderDate = item.Courses.ElementAt(2).OrderDate,
                        Status = Status.Approved,
                        Courses = new List<CourseEntry>
                        {
                            new CourseEntry
                            {
                                CourseId = item.Courses.ElementAt(2).Id,
                            },
                        }
                    },
                },
                Courses = new List<MappedCourse>
                {
                    new MappedCourse
                    {
                        Details = $"{item.Courses.ElementAt(0).Dosage} ‐ {item.Courses.ElementAt(0).Quantity}",
                        Id = item.Courses.ElementAt(0).Id,
                        Name = item.Courses.ElementAt(0).Name,
                    },
                    new MappedCourse
                    {
                        Details = $"{item.Courses.ElementAt(1).Dosage} ‐ {item.Courses.ElementAt(1).Quantity}",
                        Id = item.Courses.ElementAt(1).Id,
                        Name = item.Courses.ElementAt(1).Name,
                    },
                    new MappedCourse
                    {
                        Details = $"{item.Courses.ElementAt(2).Dosage} ‐ {item.Courses.ElementAt(2).Quantity}",
                        Id = item.Courses.ElementAt(2).Id,
                        Name = item.Courses.ElementAt(2).Name,
                    },
                }
            };

            result.Should().NotBeNull();
            result.Prescriptions.Should().HaveCount(item.Courses.Count());
            result.Courses.Should().HaveCount(item.Courses.Count());
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void MapCoursesGetResponseToCourseListResponse_WhenPassingNull_ThrowsNullReferenceException()
        {
            // Act
            Action act = () => _mapper.Map((CoursesGetResponse)null);

            // Assert
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("courseGetResponse");
        }

        [TestMethod]
        public void MapCoursesGetResponseToCourseListResponse_ReturnsResultWithSpecialRequestCharacterLimitOf1000()
        {
            // Arrange
            var item = new CoursesGetResponse();

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.SpecialRequestCharacterLimit.Should().Be(1000);
        }

        [TestMethod]
        public void MapCoursesGetResponseToCourseListResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new CoursesGetResponse();

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Courses.Should().BeEmpty();
        }

        [TestMethod]
        public void MapCoursesGetResponseToCourseListResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var item = new CoursesGetResponse()
            {
                Courses = new List<Course>
                {
                    new Course
                    {
                        Id = _fixture.Create<string>(),
                        Name = _fixture.Create<string>(),
                        Quantity = _fixture.Create<string>(),
                        Dosage = _fixture.Create<string>(),
                        Status = PrescriptionType.Repeat,
                        DoseNumber = _fixture.Create<string>(),
                    }
                }
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Courses.Should().HaveCount(item.Courses.Count());

            var expectedResult = new CourseListResponse
            {
                Courses = new List<MappedCourse>
                {
                    new MappedCourse
                    {
                        Id = item.Courses.ElementAt(0).Id,
                        Name = item.Courses.ElementAt(0).Name,
                        Details = $"{item.Courses.ElementAt(0).Dosage} ‐ {item.Courses.ElementAt(0).Quantity}",
                    }
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void MapPrescriptionOrderPartiallySuccessfulResponse_ToPrescriptionRequestPostPartialSuccessResponse_MapsCorrectly()
        {
            // Arrange
            var response = new PrescriptionOrderResponse()
            {
                PatientRequests = new List<PatientRequest>
                {
                    new PatientRequest
                    {
                        Id = _fixture.Create<string>(),
                        Name = _fixture.Create<string>(),
                        Status = PrescriptionOrderItemRequestStatus.Success,
                        RequestOutcome = _fixture.Create<string>(),
                    },
                    new PatientRequest
                    {
                        Id = _fixture.Create<string>(),
                        Name = _fixture.Create<string>(),
                        Status = PrescriptionOrderItemRequestStatus.Failed,
                        RequestOutcome = _fixture.Create<string>(),
                    },
                    new PatientRequest
                    {
                        Id = _fixture.Create<string>(),
                        Name = _fixture.Create<string>(),
                        Status = "not valid status",
                        RequestOutcome = _fixture.Create<string>(),
                    },
                }
            };

            // Act
            var result = _mapper.Map(response);

            // Assert
            result.Should().NotBeNull();
            result.SuccessfulOrders.Should().HaveCount(1);
            result.UnsuccessfulOrders.Should().HaveCount(1);

            var expectedResult = new PrescriptionRequestPostPartialSuccessResponse
            {
                SuccessfulOrders = new List<Order>
                {
                    new Order
                    {
                        CourseId = response.PatientRequests.ElementAt(0).Id,
                        Name = response.PatientRequests.ElementAt(0).Name,
                    },
                },
                UnsuccessfulOrders = new List<Order>
                {
                    new Order
                    {
                        CourseId = response.PatientRequests.ElementAt(1).Id,
                        Name = response.PatientRequests.ElementAt(1).Name,
                    },
                },
            };

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
