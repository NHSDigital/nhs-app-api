using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Prescriptions;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Prescriptions
{
    [TestClass]
    public class EmisPrescriptionMapperTests
    {
        private IFixture _fixture;
        private IEmisPrescriptionMapper _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new EmisPrescriptionMapper();

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public void MapPrescriptionRequestsGetResponseToPrescriptionListResponse_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapper.Map((PrescriptionRequestsGetResponse)null);
            
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("prescriptionGetResponse");
        }

        [TestMethod]
        public void MapPrescriptionRequestsGetResponseToPrescriptionListResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new PrescriptionRequestsGetResponse();

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Prescriptions.Should().BeEmpty();
            result.Courses.Should().BeEmpty();
        }

        [TestMethod]
        public void MapPrescriptionRequestsGetResponseToPrescriptionListResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var item = new PrescriptionRequestsGetResponse
            {
                PrescriptionRequests = new List<PrescriptionRequest>
                {
                    new PrescriptionRequest
                    {
                        DateRequested = _fixture.Create<DateTimeOffset>(),
                        RequestedMedicationCourses = new List<RequestedMedicationCourse>
                        {
                            new RequestedMedicationCourse
                            {
                                RequestedMedicationCourseStatus = RequestedMedicationCourseStatus.Issued,
                                RequestedMedicationCourseGuid = Guid.NewGuid().ToString(),
                            }
                        },
                    },
                },
                MedicationCourses = new List<MedicationCourse>
                {
                    new MedicationCourse
                    {
                        CanBeRequested = true,
                        Constituents = new List<string>
                        {
                            _fixture.Create<string>()
                        },
                        Dosage = _fixture.Create<string>(),
                        MedicationCourseGuid = Guid.NewGuid().ToString(),
                        QuantityRepresentation = _fixture.Create<string>(),
                        Name = _fixture.Create<string>(),
                        PrescriptionType = PrescriptionType.Automatic,
                    }
                }
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Prescriptions.Should().HaveCount(item.PrescriptionRequests.Count());
            result.Courses.Should().HaveCount(item.MedicationCourses.Count());

            var expectedResult = new PrescriptionListResponse
            {
                Prescriptions = new List<PrescriptionItem>
                {
                    new PrescriptionItem
                    {
                        OrderDate = item.PrescriptionRequests.ElementAt(0).DateRequested,
                        Courses = new List<CourseEntry>
                        {
                            new CourseEntry
                            {
                                CourseId = item.PrescriptionRequests.ElementAt(0).RequestedMedicationCourses.ElementAt(0).RequestedMedicationCourseGuid,
                            }
                        }
                    }
                },
                Courses = new List<Course>
                {
                    new Course
                    {
                        Dosage = item.MedicationCourses.ElementAt(0).Dosage,
                        Id = item.MedicationCourses.ElementAt(0).MedicationCourseGuid,
                        Name = item.MedicationCourses.ElementAt(0).Name,
                        Quantity = item.MedicationCourses.ElementAt(0).QuantityRepresentation,
                    }
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void MapCoursesGetResponseToCourseListResponse_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapper.Map((CoursesGetResponse)null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("coursesGetResponse");
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
                Courses = new List<MedicationCourse>
                {
                    new MedicationCourse
                    {
                        CanBeRequested = true,
                        Constituents = new List<string>
                        {
                            _fixture.Create<string>()
                        },
                        Dosage = _fixture.Create<string>(),
                        MedicationCourseGuid = Guid.NewGuid().ToString(),
                        QuantityRepresentation = _fixture.Create<string>(),
                        Name = _fixture.Create<string>(),
                        PrescriptionType = PrescriptionType.Automatic,
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
                Courses = new List<Course>
                {
                    new Course
                    {
                        Dosage = item.Courses.ElementAt(0).Dosage,
                        Id = item.Courses.ElementAt(0).MedicationCourseGuid,
                        Name = item.Courses.ElementAt(0).Name,
                        Quantity = item.Courses.ElementAt(0).QuantityRepresentation,
                    }
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }

    }
}
