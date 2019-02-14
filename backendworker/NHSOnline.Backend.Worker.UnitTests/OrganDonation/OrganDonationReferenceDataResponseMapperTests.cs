using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Worker.UnitTests.OrganDonation
{
    [TestClass]
    public class OrganDonationReferenceDataResponseMapperTests
    {
        private IFixture _fixture;

        private IMapper<OrganDonationResponse<ReferenceDataResponse>, OrganDonationReferenceDataResponse>
            _referenceDataMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _referenceDataMapper = _fixture.Create<OrganDonationReferenceDataResponseMapper>();
        }

        [TestMethod]
        public void MapResponseToOrganDonationReferenceData_WhenPassingNull_ThrowsArgumentNullException()
        {
            // Act and Assert
            Action act = () => _referenceDataMapper.Map(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("source");
        }

        [TestMethod]
        public void MapResponseToOrganDonationReferenceData_WhenPassingNullBody_ThrowsArgumentNullException()
        {
            // Arrange
            var response = new OrganDonationResponse<ReferenceDataResponse>(HttpStatusCode.OK)
            {
                Body = null
            };

            // Act and Assert
            Action act = () => _referenceDataMapper.Map(response);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("source");
        }

        [TestMethod]
        public void MapResponseToOrganDonationReferenceData_WhenPassingNullListOfEntries_ThrowsArgumentNullException()
        {
            // Arrange
            var response = new OrganDonationResponse<ReferenceDataResponse>(HttpStatusCode.OK)
            {
                Body = new ReferenceDataResponse
                {
                    Entry = null
                }
            };

            // Act and Assert
            Action act = () => _referenceDataMapper.Map(response);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("source");
        }

        [TestMethod]
        public void MapResponseToOrganDonationReferenceData_WithAllValues_MapsCorrectly()
        {
            // Arrange
            var titles = _fixture.Create<Dictionary<string, string>>();
            var religions = _fixture.Create<Dictionary<string, string>>();
            var ethnicities = _fixture.Create<Dictionary<string, string>>();
            var genders = _fixture.Create<Dictionary<string, string>>();
            var withdrawReasons = _fixture.Create<Dictionary<string, string>>();
            var response = new OrganDonationResponse<ReferenceDataResponse>(HttpStatusCode.OK)
            {
                Body = new ReferenceDataResponse
                {
                    Entry = new List<Entry<ReferenceData>>
                    {
                        CreateEntry("titles", titles),
                        CreateEntry("religions", religions),
                        CreateEntry("ethnicities", ethnicities),
                        CreateEntry("genders", genders),
                        CreateEntry("withdraw-reasons", withdrawReasons)
                    }
                }
            };

            // Act
            var result = _referenceDataMapper.Map(response);

            // Assert
            result.Should().NotBeNull();
            result.Titles.Should().NotBeNull();
            result.Titles.Should().HaveCount(titles.Count);
            result.Titles.Should().OnlyContain(x => titles.ContainsKey(x.Id)
                                                    && string.Equals(titles[x.Id], x.DisplayName,
                                                        StringComparison.Ordinal));
            result.Ethnicities.Should().NotBeNull();
            result.Ethnicities.Should().HaveCount(ethnicities.Count);
            result.Ethnicities.Should().OnlyContain(x => ethnicities.ContainsKey(x.Id)
                                                         && string.Equals(ethnicities[x.Id], x.DisplayName,
                                                             StringComparison.Ordinal));
            result.Religions.Should().NotBeNull();
            result.Religions.Should().HaveCount(religions.Count);
            result.Religions.Should().OnlyContain(x => religions.ContainsKey(x.Id)
                                                       && string.Equals(religions[x.Id], x.DisplayName,
                                                           StringComparison.Ordinal));
            result.Genders.Should().NotBeNull();
            result.Genders.Should().HaveCount(genders.Count);
            result.Genders.Should().OnlyContain(x => genders.ContainsKey(x.Id)
                                                     && string.Equals(genders[x.Id], x.DisplayName,
                                                         StringComparison.Ordinal));
            result.WithdrawReasons.Should().NotBeNull();
            result.WithdrawReasons.Should().HaveCount(withdrawReasons.Count);
            result.WithdrawReasons.Should().OnlyContain(x => withdrawReasons.ContainsKey(x.Id)
                                                             && string.Equals(withdrawReasons[x.Id], x.DisplayName,
                                                                 StringComparison.Ordinal));
        }

        [TestMethod]
        public void MapResponseToOrganDonationReferenceData_WithNoKnownEntries_MapsCorrectly()
        {
            // Arrange
            var response = new OrganDonationResponse<ReferenceDataResponse>(HttpStatusCode.OK)
            {
                Body = new ReferenceDataResponse
                {
                    Entry = new List<Entry<ReferenceData>>
                    {
                        CreateEntry("_invalid_", _fixture.Create<Dictionary<string, string>>())
                    }
                }
            };

            // Act
            var result = _referenceDataMapper.Map(response);

            // Assert
            result.Should().NotBeNull();
            result.Titles.Should().NotBeNull().And.HaveCount(0);
            result.Ethnicities.Should().NotBeNull().And.HaveCount(0);
            result.Genders.Should().NotBeNull().And.HaveCount(0);
            result.Religions.Should().NotBeNull().And.HaveCount(0);
            result.WithdrawReasons.Should().NotBeNull().And.HaveCount(0);
        }

        private Entry<ReferenceData> CreateEntry(string key, Dictionary<string, string> options)
        {
            return new Entry<ReferenceData>
            {
                Resource = new ReferenceData
                {
                    Id = key,
                    Concept = options.Select(x => new Coding { Code = x.Key, Display = x.Value })
                        .ToList()
                }
            };
        }
    }
}