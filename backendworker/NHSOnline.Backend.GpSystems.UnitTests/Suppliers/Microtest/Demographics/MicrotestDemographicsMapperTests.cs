using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;
using NHSOnline.Backend.Support;
using UnitTestHelper;

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

            _fixture.Customize<DemographicsData>(c => c.With(x => x.Nhs, _fixture.CreateNhsNumberUnformatted()));
        }

        [TestMethod]
        public void Map_DemographicsGetResponse_To_DemographicsResponse_When_Passing_Null_Throws_ArgumentNullException()
        {
            Action act = () => _mapper.Map(null);
            
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("demographicsGetResponse");
        }
        
        [TestMethod]
        public void Map_DemographicsGetResponse_To_DemographicsResponse_When_DemographicsObjectNull_Throws_ArgumentException()
        {
            // Arrange
            var response = new DemographicsGetResponse();
            
            // Act
            Action act = () => _mapper.Map(response);
            
            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("demographics object is null*")
                .And.ParamName.Should().Be("demographicsGetResponse");
        }

        [TestMethod]
        public void Map_DemographicsGetResponse_To_DemographicsResponse_With_Empty_Values_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var response = new DemographicsGetResponse
            {
                Demographics = new DemographicsData()
            };

            // Act
            var result = _mapper.Map(response);

            // Assert
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void Map_DemographicsGetResponse_To_DemographicsResponse_With_Values_Returns_Result_Value()
        {
            // Arrange
            var response = _fixture.Create<DemographicsGetResponse>();
            
            var demographics = response.Demographics;
            
            var expectedResult = new DemographicsResponse
            {
                PatientName = $"{demographics.Title} {demographics.Forenames1} {demographics.Forenames2} {demographics.Surname}",
                DateOfBirth = demographics.Dob,
                Sex = demographics.Sex,
                NhsNumber = demographics.Nhs.FormatToNhsNumber(),
                Address = $"{demographics.HouseName}, {demographics.RoadName}, {demographics.Locality}, " +
                          $"{demographics.PostTown}, {demographics.County}, {demographics.Postcode}",
                AddressParts = new DemographicsAddress
                {
                    Text = $"{demographics.HouseName}, {demographics.RoadName}, {demographics.Locality}, " +
                           $"{demographics.PostTown}, {demographics.County}",
                    Postcode = demographics.Postcode
                },
                NameParts = new DemographicsName
                {
                    Title = demographics.Title,
                    Given = demographics.Forenames1,
                    Surname = demographics.Surname
                }
            };

            // Act
            var result = _mapper.Map(response);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [DataTestMethod]
        [DataRow("1234567890", "123 456 7890")]
        [DataRow("9852746152", "985 274 6152")]
        [DataRow(" 98  52 74  6152 ", "985 274 6152")]
        [DataRow(null, "")]
        [DataRow("", "")]
        public void Map_DemographicsGetResponse_To_DemographicsResponse_NhsNumberMappingTests(string responseNhsNumber, string expectedNhsNumber)
        {
            // Arrange
            var response = _fixture.Create<DemographicsGetResponse>();
            response.Demographics.Nhs = responseNhsNumber;

            // Act
            var result = _mapper.Map(response);

            // Assert
            result.NhsNumber.Should().BeEquivalentTo(expectedNhsNumber);
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
            var response = _fixture.Create<DemographicsGetResponse>();
            response.Demographics.Title = title;
            response.Demographics.Forenames1 = forename1;
            response.Demographics.Forenames2 = forename2;
            response.Demographics.Surname = surname;
            
            var expectedNameParts = new DemographicsName
            {
                Title = title,
                Given = forename1,
                Surname = surname
            };

            // Act
            var result = _mapper.Map(response);

            // Assert
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
            var response = _fixture.Create<DemographicsGetResponse>();
            response.Demographics.HouseName = houseName;
            response.Demographics.RoadName = roadName;
            response.Demographics.Locality = locality;
            response.Demographics.PostTown = postTown;
            response.Demographics.County = county;
            response.Demographics.Postcode = postcode;

            // Act
            var result = _mapper.Map(response);

            // Assert
            result.Address.Should().BeEquivalentTo(expectedAddress);
        }
    }
}
