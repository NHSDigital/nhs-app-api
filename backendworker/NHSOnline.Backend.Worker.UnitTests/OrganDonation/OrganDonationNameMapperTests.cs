using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Worker.Support;
using Name = NHSOnline.Backend.Worker.OrganDonation.ApiModels.Name;

namespace NHSOnline.Backend.Worker.UnitTests.OrganDonation
{
    [TestClass]
    public class OrganDonationNameMapperTests
    {
        private IMapper<string, NHSOnline.Backend.Worker.OrganDonation.Models.Name, Name>
            _organDonationNameMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _organDonationNameMapper = fixture.Create<OrganDonationNameMapper>();
        }

        [TestMethod]
        public void MapToOrganDonationName_WhenPassingNoFullNameAndNoSplitName_ThrowsAnException()
        {
            // Act and Assert
            Action act = () => _organDonationNameMapper.Map(null, null);

            act.Should().Throw<ArgumentException>();
        }


        [TestMethod]
        [DataRow("")]
        [DataRow("           ")]
        public void MapToOrganDonationName_WhenPassingEmptyFullNameAndNoSplitName_ThrowsAnException(string fullName)
        {
            // Act and Assert
            Action act = () => _organDonationNameMapper.Map(fullName, null);

            act.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void MapToOrganDonationName_WhenPassingLongFullName_ParsesFullName()
        {
            // Act 
            var result = _organDonationNameMapper.Map("Mr John Johnson Stealth-Smith", null);

            // Assert
            result.Should().NotBeNull();
            result.Family.Should().Be("Stealth-Smith");
            result.Prefix.Should().ContainSingle().Which.Should().Be("MR");
            result.Given.Should().ContainSingle().Which.Should().Be("John Johnson");
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
        public void MapToOrganDonationName_WhenPassingFullName_ParsesFullName(string title, string mappedTitle)
        {
            // Act 
            var result = _organDonationNameMapper.Map($"{title} John Smith", null);

            // Assert
            result.Should().NotBeNull();
            result.Family.Should().Be("Smith");
            result.Prefix.Should().ContainSingle().Which.Should().Be(mappedTitle);
            result.Given.Should().ContainSingle().Which.Should().Be("John");
        }

        [TestMethod]
        public void MapToOrganDonationName_WhenPassingFullNameWithOnlyTitle_ParsesFullName()
        {
            // Act 
            var result = _organDonationNameMapper.Map("Mr", null);

            // Assert
            result.Should().NotBeNull();
            result.Family.Should().Be("Mr");
            result.Prefix.Should().BeNull();
            result.Given.Should().BeNull();
        }
        
        [TestMethod]
        public void MapToOrganDonationName_WhenPassingFullNameWithOnlyTitleAndSurname_ParsesFullName()
        {
            // Act 
            var result = _organDonationNameMapper.Map("Mr Smith", null);

            // Assert
            result.Should().NotBeNull();
            result.Family.Should().Be("Smith");
            result.Prefix.Should().BeNull();
            result.Given.Should().ContainSingle().Which.Should().Be("MR");
        }

        [TestMethod]
        public void MapToOrganDonationName_WhenPassingFullNameWithOnlySurname_ParsesFullName()
        {
            // Act 
            var result = _organDonationNameMapper.Map("Smith", null);

            // Assert
            result.Should().NotBeNull();
            result.Family.Should().Be("Smith");
            result.Prefix.Should().BeNull();
            result.Given.Should().BeNull();
        }

        [TestMethod]
        public void MapToOrganDonationName_WhenPassingFullNameWithNoTitle_ParsesFullName()
        {
            // Act 
            var result = _organDonationNameMapper.Map("John Smith", null);

            // Assert
            result.Should().NotBeNull();
            result.Family.Should().Be("Smith");
            result.Prefix.Should().BeNull();
            result.Given.Should().ContainSingle().Which.Should().Be("John");
        }
        
        [TestMethod]
        public void MapToOrganDonationName_WhenPassingBothNames_UseSplitName()
        {
            // Act 
            var result = _organDonationNameMapper.Map("MR John Smith",
                new NHSOnline.Backend.Worker.OrganDonation.Models.Name
                {
                    GivenName = "Joanne",
                    Surname = "Beanstalk",
                    Title = "Mrs"
                });

            // Assert
            result.Should().NotBeNull();
            result.Family.Should().Be("Beanstalk");
            result.Prefix.Should().ContainSingle().Which.Should().Be("MRS");
            result.Given.Should().ContainSingle().Which.Should().Be("Joanne");
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
            var result = _organDonationNameMapper.Map(null,
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
            var result = _organDonationNameMapper.Map(null,
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
        public void MapToOrganDonationName_WhenPassingSplitNameWithNoFirstName_MapTheRemainingFields()
        {
            // Act 
            var result = _organDonationNameMapper.Map(null,
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
            var result = _organDonationNameMapper.Map(null,
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