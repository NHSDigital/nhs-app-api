using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Support;
using Name = NHSOnline.Backend.Worker.OrganDonation.ApiModels.Name;

namespace NHSOnline.Backend.Worker.UnitTests.OrganDonation
{
    [TestClass]
    public class OrganDonationNameMapperTests
    {
        private IMapper<NHSOnline.Backend.Worker.OrganDonation.Models.Name, Name>
            _organDonationNameMapper;
        private Mock<IOrganDonationDataMaps> _dataMaps;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            _dataMaps = fixture.Freeze<Mock<IOrganDonationDataMaps>>();

            _dataMaps
                .Setup(x => x.TitleDataMap)
                .Returns(new Dictionary<string, string>
                {
                    { "MR", "MR" },
                    { "MRS", "MRS" },
                    { "MISS", "MISS" },
                    { "MS", "MS" },
                    { "MX", "MX" },
                    { "MASTER", "MASTER" },
                    { "DOCTOR", "DR" },
                    { "DR", "DR" },
                    { "CLLR", "CLLR" },
                    { "COUNCILLOR", "CLLR" },
                    { "CAPT", "CAPT" },
                    { "CAPTAIN", "CAPT" },
                    { "COLONEL", "COLONEL" },
                    { "EXORS", "EXORS" },
                    { "EXECUTORS OF", "EXORS" },
                    { "FR", "FR" },
                    { "FATHER", "FR" },
                    { "LADY", "LADY" },
                    { "LORD", "LORD" },
                    { "PROFESSOR", "PROF" },
                    { "REV", "REV" },
                    { "REVEREND", "REV" },
                    { "SIR", "SIR" },
                    { "DAME", "DAME" }
                });

            _organDonationNameMapper = fixture.Create<OrganDonationNameMapper>();
        }

        [TestMethod]
        public void MapToOrganDonationName_WhenPassingNoSplitName_ThrowsAnException()
        {
            // Act and Assert
            Action act = () => _organDonationNameMapper.Map(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow("Mr", "MR")]
        [DataRow("Mrs", "MRS")]
        [DataRow("Miss", "MISS")]
        [DataRow("Ms", "MS")]
        [DataRow("Mx", "MX")]
        [DataRow("Master", "MASTER")]
        [DataRow("Doctor", "DR")]
        [DataRow("Dr", "DR")]
        [DataRow("Cllr", "CLLR")]
        [DataRow("Councillor", "CLLR")]
        [DataRow("Capt", "CAPT")]
        [DataRow("Captain", "CAPT")]
        [DataRow("Colonel", "COLONEL")]
        [DataRow("Exors", "EXORS")]
        [DataRow("Executors of", "EXORS")]
        [DataRow("Fr", "FR")]
        [DataRow("Father", "FR")]
        [DataRow("Lady", "LADY")]
        [DataRow("Lord", "LORD")]
        [DataRow("Professor", "PROF")]
        [DataRow("Rev", "REV")]
        [DataRow("Reverend", "REV")]
        [DataRow("Sir", "SIR")]
        [DataRow("Dame", "DAME")]
        public void MapToOrganDonationName_WhenPassingSplitName_MapTheFields(string title, string mappedTitle)
        {
            // Act 
            var result = _organDonationNameMapper.Map(
                new NHSOnline.Backend.Worker.OrganDonation.Models.Name
                {
                    GivenName = "Joanne",
                    Surname = "Beanstalk",
                    Title = title
                });

            // Assert
            result.Should().NotBeNull();
            result.Family.Should().Be("Beanstalk");
            result.Prefix.Should().ContainSingle().Which.Should().Be(mappedTitle);
            result.Given.Should().ContainSingle().Which.Should().Be("Joanne");
        }
        
        [TestMethod]
        public void MapToOrganDonationName_WhenPassingSplitNameWithNoTitle_MapTheRemainingFields()
        {
            // Act 
            var result = _organDonationNameMapper.Map(
                new NHSOnline.Backend.Worker.OrganDonation.Models.Name
                {
                    GivenName = "Joanne",
                    Surname = "Beanstalk",
                    Title = null
                });

            // Assert
            result.Should().NotBeNull();
            result.Family.Should().Be("Beanstalk");
            result.Prefix.Should().BeNull();
            result.Given.Should().ContainSingle().Which.Should().Be("Joanne");
        }
        
        [TestMethod]
        public void MapToOrganDonationName_WhenPassingSplitNameWithUnMappedTitle_MapsTheTitleToGivenAndRemainingFields()
        {
            // Act 
            var result = _organDonationNameMapper.Map(
                new NHSOnline.Backend.Worker.OrganDonation.Models.Name
                {
                    GivenName = "Joanne",
                    Surname = "Beanstalk",
                    Title = "SomeThing"
                });

            // Assert
            result.Should().NotBeNull();
            result.Family.Should().Be("Beanstalk");
            result.Prefix.Should().BeNull();
            result.Given.Should().ContainSingle().Which.Should().Be("SomeThing Joanne");
        }
        
        [TestMethod]
        public void MapToOrganDonationName_WhenPassingSplitNameWithNoFirstName_MapTheRemainingFields()
        {
            // Act 
            var result = _organDonationNameMapper.Map(
                new NHSOnline.Backend.Worker.OrganDonation.Models.Name
                {
                    GivenName = null,
                    Surname = "Beanstalk",
                    Title = "Mrs"
                });

            // Assert
            result.Should().NotBeNull();
            result.Family.Should().Be("Beanstalk");
            result.Prefix.Should().ContainSingle().Which.Should().Be("MRS");
            result.Given.Should().BeNull();
        }
        
        [TestMethod]
        public void MapToOrganDonationName_WhenPassingSplitNameWithNoSurname_MapTheRemainingFields()
        {
            // Act 
            var result = _organDonationNameMapper.Map(
                new NHSOnline.Backend.Worker.OrganDonation.Models.Name
                {
                    GivenName = "Joanne",
                    Surname = null,
                    Title = "Mrs"
                });

            // Assert
            result.Should().NotBeNull();
            result.Family.Should().BeNull();
            result.Prefix.Should().ContainSingle().Which.Should().Be("MRS");
            result.Given.Should().ContainSingle().Which.Should().Be("Joanne");
        }
    }
}