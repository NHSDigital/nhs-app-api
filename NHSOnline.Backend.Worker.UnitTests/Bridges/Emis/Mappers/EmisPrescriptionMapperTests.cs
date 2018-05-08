using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Mappers;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis.Mappers
{
    [TestClass]
    public class EmisPrescriptionMapperTests
    {
        private static IFixture _fixture;

        IEmisPrescriptionMapper _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new EmisPrescriptionMapper();

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public void Map_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapper.Map(null);
            
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("prescriptionGetResponse");
        }

        [TestMethod]
        public void Map_WithEmptyValues_ReturnsResultWithEmptyValues()
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
        public void Map_WithValues_ReturnsResultValues()
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
    }
}
