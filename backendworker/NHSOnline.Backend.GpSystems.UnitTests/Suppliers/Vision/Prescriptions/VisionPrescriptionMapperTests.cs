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
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Prescriptions;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Prescriptions
{
    [TestClass]
    public class VisionPrescriptionMapperTests
    {
        private IFixture _fixture;
        private IVisionPrescriptionMapper _mapper;
        private ILogger<VisionPrescriptionMapper> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = Mock.Of<ILogger<VisionPrescriptionMapper>>();
            _mapper = new VisionPrescriptionMapper(_logger);

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public void MapPrescriptionHistoryToPrescriptionListResponse_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapper.Map((PrescriptionHistory)null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("prescriptionGetResponse");
        }

        [TestMethod]
        public void MapPrescriptionHistoryToPrescriptionListResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new PrescriptionHistory();

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Prescriptions.Should().BeEmpty();
            result.Courses.Should().BeEmpty();
        }

        [TestMethod]
        public void MapPrescriptionHistoryToPrescriptionListResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var item = new PrescriptionHistory
            {
                Requests = new List<Request>
                {
                    new Request
                    {
                        Date = new DateTime(2020, 1, 1, 12, 12, 12, DateTimeKind.Utc),
                        Status = new Backend.GpSystems.Suppliers.Vision.Models.Prescriptions.Status
                        {
                            Code = PrescriptionRepeatStatusCode.Processed,
                            Text = "Processed",
                        },
                        Repeats = new List<GetPrescriptionRepeat>
                        {
                            new GetPrescriptionRepeat
                            {
                                Drug = _fixture.Create<string>(),
                                Dosage = _fixture.Create<string>(),
                                Quantity = _fixture.Create<string>(),
                            },
                            new GetPrescriptionRepeat
                            {
                                // only dosage, no quantity
                                Drug = _fixture.Create<string>(),
                                Dosage = _fixture.Create<string>(),
                            },
                        },
                    },
                    new Request
                    {
                        Date = new DateTime(2020, 1, 2, 12, 12, 12, DateTimeKind.Utc),
                        Status = new Backend.GpSystems.Suppliers.Vision.Models.Prescriptions.Status
                        {
                            Code = PrescriptionRepeatStatusCode.Rejected,
                            Text = "Rejected",
                        },
                        Repeats = new List<GetPrescriptionRepeat>
                        {
                            new GetPrescriptionRepeat
                            {
                                // only quantity, no dosage
                                Drug = _fixture.Create<string>(),
                                Quantity = _fixture.Create<string>(),
                            },
                           new GetPrescriptionRepeat
                            {
                                // no dosage or quantity
                                Drug = _fixture.Create<string>(),
                            },
                        },
                    },
                    new Request
                    {
                        Date = new DateTime(2020, 1, 3, 12, 12, 12, DateTimeKind.Utc),
                        Status = new Backend.GpSystems.Suppliers.Vision.Models.Prescriptions.Status
                        {
                            Code = PrescriptionRepeatStatusCode.InProgress,
                            Text = "In Progress",
                        },
                        Repeats = new List<GetPrescriptionRepeat>
                        {
                            new GetPrescriptionRepeat(),
                        },
                    },
                    new Request
                    {
                        Date = new DateTime(2020, 1, 4, 12, 12, 12, DateTimeKind.Utc),
                        Status = new Backend.GpSystems.Suppliers.Vision.Models.Prescriptions.Status
                        {
                            Code = PrescriptionRepeatStatusCode.NotProcessed,
                            Text = "Not Processed",
                        },
                        Repeats = new List<GetPrescriptionRepeat>
                        {
                            new GetPrescriptionRepeat(),
                        },
                    },
                },
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Prescriptions.Should().HaveCount(4);
            result.Courses.Should().HaveCount(6);

            var expectedResult = new PrescriptionListResponse
            {
                Prescriptions = new List<PrescriptionItem>
                {
                    new PrescriptionItem
                    {
                        OrderDate = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        Status = Backend.GpSystems.Prescriptions.Models.Status.Approved,
                        Courses = new List<CourseEntry>
                        {
                            new CourseEntry
                            {
                                CourseId = result.Courses.ElementAt(0).Id, // id is generated by mapper so use value from result
                            },
                            new CourseEntry
                            {
                                CourseId = result.Courses.ElementAt(1).Id, // id is generated by mapper so use value from result
                            },
                        }
                    },
                    new PrescriptionItem
                    {
                        OrderDate = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                        Status = Backend.GpSystems.Prescriptions.Models.Status.Rejected,
                        Courses = new List<CourseEntry>
                        {
                            new CourseEntry
                            {
                                CourseId = result.Courses.ElementAt(2).Id, // id is generated by mapper so use value from result
                            },
                            new CourseEntry
                            {
                                CourseId = result.Courses.ElementAt(3).Id, // id is generated by mapper so use value from result
                            },
                        }
                    },
                    new PrescriptionItem
                    {
                        OrderDate = new DateTime(2020, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                        Status = Backend.GpSystems.Prescriptions.Models.Status.Requested,
                        Courses = new List<CourseEntry>
                        {
                            new CourseEntry
                            {
                                CourseId = result.Courses.ElementAt(4).Id, // id is generated by mapper so use value from result
                            },
                        }
                    },
                    new PrescriptionItem
                    {
                        OrderDate = new DateTime(2020, 1, 4, 0, 0, 0, DateTimeKind.Utc),
                        Status = Backend.GpSystems.Prescriptions.Models.Status.Requested,
                        Courses = new List<CourseEntry>
                        {
                            new CourseEntry
                            {
                                CourseId = result.Courses.ElementAt(5).Id, // id is generated by mapper so use value from result
                            },
                        }
                    },
                },
                Courses = new List<Course>
                {
                    new Course
                    {
                        Details = $"{item.Requests.ElementAt(0).Repeats.ElementAt(0).Dosage} ‐ {item.Requests.ElementAt(0).Repeats.ElementAt(0).Quantity}",
                        Id = result.Courses.ElementAt(0).Id, // id is generated by mapper so use value from result
                        Name = item.Requests.ElementAt(0).Repeats.ElementAt(0).Drug,
                    },
                    new Course
                    {
                        Details = $"{item.Requests.ElementAt(0).Repeats.ElementAt(1).Dosage}",
                        Id = result.Courses.ElementAt(1).Id, // id is generated by mapper so use value from result
                        Name = item.Requests.ElementAt(0).Repeats.ElementAt(1).Drug,
                    },
                    new Course
                    {
                        Details = $"{item.Requests.ElementAt(1).Repeats.ElementAt(0).Quantity}",
                        Id = result.Courses.ElementAt(2).Id, // id is generated by mapper so use value from result
                        Name = item.Requests.ElementAt(1).Repeats.ElementAt(0).Drug,
                    },
                    new Course
                    {
                        Details = null,
                        Id = result.Courses.ElementAt(3).Id, // id is generated by mapper so use value from result
                        Name = item.Requests.ElementAt(1).Repeats.ElementAt(1).Drug,
                    },
                    new Course
                    {
                        Id = result.Courses.ElementAt(4).Id, // id is generated by mapper so use value from result
                    },
                    new Course
                    {
                        Id = result.Courses.ElementAt(5).Id, // id is generated by mapper so use value from result
                    },
                }
            };

            expectedResult.Should().BeEquivalentTo(result);
        }

        [TestMethod]
        public void MapPrescriptionHistoryToPrescriptionListResponse_MultiplePrescriptionsWithSameDateAndStatus_ShouldBeGroupedTogether()
        {
            // Arrange
            var today = DateTime.Today;

            var item = new PrescriptionHistory
            {
                Requests = new List<Request>
                {
                    new Request
                    {
                        Date = today,
                        Status = new Backend.GpSystems.Suppliers.Vision.Models.Prescriptions.Status
                        {
                            Code = PrescriptionRepeatStatusCode.Processed,
                            Text = "Processed",
                        },
                        Repeats = new List<GetPrescriptionRepeat>
                        {
                            new GetPrescriptionRepeat { Drug = _fixture.Create<string>() },
                            new GetPrescriptionRepeat { Drug = _fixture.Create<string>() },
                        },
                    },
                    new Request
                    {
                        Date = today,
                        Status = new Backend.GpSystems.Suppliers.Vision.Models.Prescriptions.Status
                        {
                            Code = PrescriptionRepeatStatusCode.Processed,
                            Text = "Processed",
                        },
                        Repeats = new List<GetPrescriptionRepeat>
                        {
                            new GetPrescriptionRepeat { Drug = _fixture.Create<string>() },
                            new GetPrescriptionRepeat { Drug = _fixture.Create<string>() },
                            new GetPrescriptionRepeat { Drug = _fixture.Create<string>() },
                        },
                    },
                },
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Prescriptions.Should().HaveCount(1);
            result.Courses.Should().HaveCount(item.Requests.SelectMany(x => x.Repeats).Count());

            var expectedResult = new PrescriptionListResponse
            {
                Prescriptions = new List<PrescriptionItem>
                {
                    new PrescriptionItem
                    {
                        OrderDate = today,
                        Status = Backend.GpSystems.Prescriptions.Models.Status.Approved,
                        Courses = new List<CourseEntry>
                        {
                            new CourseEntry { CourseId = result.Courses.ElementAt(0).Id },
                            new CourseEntry { CourseId = result.Courses.ElementAt(1).Id },
                            new CourseEntry { CourseId = result.Courses.ElementAt(2).Id },
                            new CourseEntry { CourseId = result.Courses.ElementAt(3).Id },
                            new CourseEntry { CourseId = result.Courses.ElementAt(4).Id },
                        },
                    },
                },
                Courses = new List<Course>
                {
                    new Course
                    {
                        Id = result.Courses.ElementAt(0).Id, // id is generated by mapper so use value from result
                        Name = item.Requests.ElementAt(0).Repeats.ElementAt(0).Drug,
                    },
                    new Course
                    {
                        Id = result.Courses.ElementAt(1).Id, // id is generated by mapper so use value from result
                        Name = item.Requests.ElementAt(0).Repeats.ElementAt(1).Drug,
                    },
                    new Course
                    {
                        Id = result.Courses.ElementAt(2).Id, // id is generated by mapper so use value from result
                        Name = item.Requests.ElementAt(1).Repeats.ElementAt(0).Drug,
                    },
                    new Course
                    {
                        Id = result.Courses.ElementAt(3).Id, // id is generated by mapper so use value from result
                        Name = item.Requests.ElementAt(1).Repeats.ElementAt(1).Drug,
                    },
                    new Course
                    {
                        Id = result.Courses.ElementAt(4).Id, // id is generated by mapper so use value from result
                        Name = item.Requests.ElementAt(1).Repeats.ElementAt(2).Drug,
                    },
                }
            };

            // Exclude missing members as CourseId is generated so can't setup expectation for it.
            expectedResult.Should().BeEquivalentTo(result);
        }

        [TestMethod]
        public void MapListRepeatsResponse_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapper.Map((EligibleRepeats)null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("eligibleRepeatsResponse");
        }

        [TestMethod]
        public void MapRepeatsToCourseListResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new EligibleRepeats();

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Courses.Should().BeEmpty();
        }

        [TestMethod]
        public void MapRepeatsToCourseListResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var item = new EligibleRepeats
            {
                Repeats = new List<Repeat>
                {
                    new Repeat
                    {
                        Drug = _fixture.Create<string>(),
                        Id = _fixture.Create<string>(),
                        Quantity = _fixture.Create<string>(),
                        Dosage = _fixture.Create<string>(),
                    },
                    new Repeat
                    {
                        Drug = _fixture.Create<string>(),
                        Id = _fixture.Create<string>(),
                        Quantity = _fixture.Create<string>(),
                        Dosage = _fixture.Create<string>(),
                    },
                },
                Settings = new CourseSettings
                {
                    AllowFreeText = true,
                },
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Courses.Should().HaveCount(item.Repeats.Count());

            var expectedResult = new CourseListResponse
            {
                Courses = new List<Course>
                {
                    new Course
                    {
                        Details = $"{item.Repeats.ElementAt(0).Dosage} ‐ {item.Repeats.ElementAt(0).Quantity}",
                        Id = item.Repeats.ElementAt(0).Id,
                        Name = item.Repeats.ElementAt(0).Drug,
                    },
                    new Course
                    {
                        Details = $"{item.Repeats.ElementAt(1).Dosage} ‐ {item.Repeats.ElementAt(1).Quantity}",
                        Id = item.Repeats.ElementAt(1).Id,
                        Name = item.Repeats.ElementAt(1).Drug,
                    },
                },
                SpecialRequestNecessity = Necessity.Optional,
            };

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void MapRepeatsToCourseListResponse_WhenAllowFreeTextIsFalse_ReturnsResultWithNecessityAsNotAllowed()
        {
            // Arrange
            var item = new EligibleRepeats
            {
                Settings = new CourseSettings
                {
                    AllowFreeText = false,
                },
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.SpecialRequestNecessity.Should().Be(Necessity.NotAllowed);
        }

        [TestMethod]
        public void MapRepeatsToCourseListResponse_WhenAllowFreeTextIsTrue_ReturnsResultWithNecessityAsOptional()
        {
            // Arrange
            var item = new EligibleRepeats
            {
                Settings = new CourseSettings
                {
                    AllowFreeText = true,
                },
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.SpecialRequestNecessity.Should().Be(Necessity.Optional);
        }
    }
}
