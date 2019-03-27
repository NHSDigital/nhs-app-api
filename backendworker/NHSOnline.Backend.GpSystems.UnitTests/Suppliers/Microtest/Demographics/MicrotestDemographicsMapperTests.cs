using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Demographics
{
    [TestClass]
    public class MicrotestDemographicsMapperTests
    {
        private IFixture _fixture;
        private IMicrotestDemographicsMapper _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new MicrotestDemographicsMapper();

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public void Map_DemographicsGetResponse_To_DemographicsResponse_When_Passing_Null_Throws_ArgumentNullException()
        {
            Action act = () => _mapper.Map(null);
            
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("demographicsGetResponse");
        }

        [TestMethod]
        public void Map_DemographicsGetResponse_To_DemographicsResponse_With_Empty_Values_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new DemographicsGetResponse();

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void Map_DemographicsGetResponse_To_DemographicsResponse_With_Values_Returns_Result_Value()
        {
            // Arrange
            var item = _fixture.Create<DemographicsGetResponse>();

            // Act
            var result = _mapper.Map(item);

            // Assert
            var expectedResult = new DemographicsResponse
            {
                PatientName = $"{item.Title} {item.Forenames1} {item.Forenames2} {item.Surname}",
                DateOfBirth = item.Dob,
                Sex = item.Sex,
                NhsNumber = item.Nhs,
                Address = $"{item.HouseName}, {item.RoadName}, {item.Locality}, " +
                          $"{item.PostTown}, {item.County}, {item.Postcode}",
                AddressParts = new DemographicsAddress
                {
                    Text = $"{item.HouseName}, {item.RoadName}, {item.Locality}, " +
                           $"{item.PostTown}, {item.County}",
                    Postcode = item.Postcode
                },
                NameParts = new DemographicsName
                {
                    Title = item.Title,
                    Given = item.Forenames1,
                    Surname = item.Surname
                },
            };

            result.Should().BeEquivalentTo(expectedResult);
        }

        [DataTestMethod]
        [DataRow("Mr", "Fred", "Freddie", "Blogs", "Mr Fred Freddie Blogs")]
        [DataRow("", "Fred", "Freddie", "Blogs", "Fred Freddie Blogs")]
        [DataRow("Mr", "", "Freddie", "Blogs", "Mr Freddie Blogs")]
        [DataRow("Mr", "Fred", "Freddie", "", "Mr Fred Freddie")]
        [DataRow("Mr", "Fred", "Blogs", "", "Mr Fred Blogs")]
        [DataRow("", "Fred", "Blogs", "", "Fred Blogs")]
        [DataRow("Mr", "", "Blogs", "", "Mr Blogs")]
        [DataRow("Mr", "Fred", "", "", "Mr Fred")]
        public void Map_DemographicsGetResponse_To_DemographicsResponse_With_MissingNameValues(string title,
            string forename1, string forename2, string surname, string expectedName)
        {
            // Arrange
            var item = new DemographicsGetResponse
            {
                Title = title,
                Forenames1 = forename1,
                Forenames2 = forename2,
                Surname = surname,
                Dob = _fixture.Create<DateTime>(),
                Sex = _fixture.Create<string>(),
                Nhs = _fixture.Create<string>(),
                HouseName = _fixture.Create<string>(),
                RoadName = _fixture.Create<string>(),
                Locality = _fixture.Create<string>(),
                PostTown = _fixture.Create<string>(),
                County = _fixture.Create<string>(),
                Postcode = _fixture.Create<string>(),
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            var expectedNameParts = new DemographicsName
            {
                Title = title,
                Given = forename1,
                Surname = surname
            };

            result.PatientName.Should().BeEquivalentTo(expectedName);
            result.NameParts.Should().BeEquivalentTo(expectedNameParts);
        }

        [DataTestMethod]
        [DataRow("The Rambles", "Little Road", "The Locality", "The Town", "County Cleveland", "AB12 3CD", "The Rambles, Little Road, The Locality, The Town, County Cleveland, AB12 3CD")]
        [DataRow("43", "Little Road", "The Locality", "The Town", "County Cleveland", "AB12 3CD", "43, Little Road, The Locality, The Town, County Cleveland, AB12 3CD")]
        [DataRow("", "43 Little Road", "The Locality", "The Town", "County Cleveland", "AB12 3CD", "43 Little Road, The Locality, The Town, County Cleveland, AB12 3CD")]
        [DataRow("The Rambles", "", "The Locality", "The Town", "County Cleveland", "AB12 3CD", "The Rambles, The Locality, The Town, County Cleveland, AB12 3CD")]
        [DataRow("The Rambles", "Little Road", "", "The Town", "County Cleveland", "AB12 3CD", "The Rambles, Little Road, The Town, County Cleveland, AB12 3CD")]
        [DataRow("The Rambles", "Little Road", "The Locality", "", "County Cleveland", "AB12 3CD", "The Rambles, Little Road, The Locality, County Cleveland, AB12 3CD")]
        [DataRow("The Rambles", "Little Road", "The Locality", "The Town", "", "AB12 3CD", "The Rambles, Little Road, The Locality, The Town, AB12 3CD")]
        [DataRow("The Rambles", "Little Road", "The Locality", "The Town", "County Cleveland", "", "The Rambles, Little Road, The Locality, The Town, County Cleveland")]

        public void Map_DemographicsGetResponse_To_DemographicsResponse_With_MissingAddressValues(
            string houseName,
            string roadName, 
            string locality, 
            string postTown, 
            string county,
            string postcode,
            string expectedAddress)
        {
            // Arrange
            var item = new DemographicsGetResponse
            {
                Title = _fixture.Create<string>(),
                Forenames1 = _fixture.Create<string>(),
                Forenames2 = _fixture.Create<string>(),
                Surname = _fixture.Create<string>(),
                Dob = _fixture.Create<DateTime>(),
                Sex = _fixture.Create<string>(),
                Nhs = _fixture.Create<string>(),
                HouseName = houseName,
                RoadName = roadName,
                Locality = locality,
                PostTown = postTown,
                County = county,
                Postcode = postcode,
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Address.Should().BeEquivalentTo(expectedAddress);
        }
    }
}
