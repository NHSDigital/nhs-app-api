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
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Courses
{
    [TestClass]
    public class TppCourseMapperTests
    {
        private IFixture _fixture;
        private ITppCourseMapper _mapper;
        private ILogger<TppCourseMapper> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = Mock.Of<ILogger<TppCourseMapper>>();
            _mapper = new TppCourseMapper(_logger);

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public void MapMedicationToCourseListResponse_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapper.Map(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("medications");
        }

        [TestMethod]
        public void MapMedicationsToCourseListResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new List<Medication>();

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Courses.Should().BeEmpty();
        }

        [TestMethod]
        public void MapMedicationsToCourseListResponse_WithValues_ReturnsResultValues()
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
            result.Courses.Should().HaveCount(2);

            var expectedResult = new CourseListResponse()
            {
                Courses = new List<Course>
                {
                    new Course
                    {
                        Id = item.ElementAt(0).DrugId,
                        Name = item.ElementAt(0).Drug,
                        Details = item.ElementAt(0).Details
                    },
                    new Course
                    {
                        Id = item.ElementAt(1).DrugId,
                        Name = item.ElementAt(1).Drug,
                        Details = item.ElementAt(1).Details
                    }
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}