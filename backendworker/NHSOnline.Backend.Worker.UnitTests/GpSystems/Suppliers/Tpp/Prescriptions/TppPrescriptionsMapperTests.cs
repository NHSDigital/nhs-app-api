using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Prescriptions
{
    [TestClass]
    public class TppPrescriptionMapperTests
    {
        private IFixture _fixture;
        private ITppPrescriptionMapper _mapper;
        private ILogger<TppPrescriptionMapper> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = Mock.Of<ILogger<TppPrescriptionMapper>>();
            _mapper = new TppPrescriptionMapper(_logger);

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public void MapMedicationToPrescriptionListResponse_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapper.Map(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("medications");
        }

        [TestMethod]
        public void MapMedicationsToPrescriptionListResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new List<Medication>();

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Prescriptions.Should().BeEmpty();
            result.Courses.Should().BeEmpty();
        }

        [TestMethod]
        public void MapMedicationsToPrescriptionListResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var item = new List<Medication>
            {
                new Medication
                {
                    Drug = _fixture.Create<string>(),
                    DrugId = Guid.NewGuid().ToString(),
                    Details = _fixture.Create<string>(),
                    Requestable = "y",
                    Type = "Repeat",
                },
                new Medication
                {
                    Drug = _fixture.Create<string>(),
                    DrugId = Guid.NewGuid().ToString(),
                    Details = _fixture.Create<string>(),
                },
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Prescriptions.Should().HaveCount(1);
            result.Courses.Should().HaveCount(item.Count());

            var expectedResult = new PrescriptionListResponse
            {
                Prescriptions = new List<PrescriptionItem>
                {
                    new PrescriptionItem
                    {
                        OrderDate = null,
                        Status = null,
                        Courses = new List<CourseEntry>
                        {
                            new CourseEntry
                            {
                                CourseId = item.ElementAt(0).DrugId,
                            },
                            new CourseEntry
                            {
                                CourseId = item.ElementAt(1).DrugId,
                            }
                        }
                    }
                },
                Courses = new List<Course>
                {
                    new Course
                    {
                        Details = item.ElementAt(0).Details,
                        Id = item.ElementAt(0).DrugId,
                        Name = item.ElementAt(0).Drug,
                    },
                    new Course
                    {
                        Details = item.ElementAt(1).Details,
                        Id = item.ElementAt(1).DrugId,
                        Name = item.ElementAt(1).Drug,
                    },
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}