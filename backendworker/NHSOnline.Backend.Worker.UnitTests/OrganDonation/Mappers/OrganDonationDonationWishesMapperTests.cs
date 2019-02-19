using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Worker.OrganDonation.Mappers;
using NHSOnline.Backend.Worker.OrganDonation.Models;

namespace NHSOnline.Backend.Worker.UnitTests.OrganDonation.Mappers
{
    [TestClass]
    public class OrganDonationDonationWishesMapperTests
    {
        private IOrganDonationDonationWishesMapper _organDonationDonationWishesMapper;
        private Mock<IEnumMapper<string, ChoiceState>> _choiceStateMapperMock;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _choiceStateMapperMock = fixture.Freeze<Mock<IEnumMapper<string, ChoiceState>>>();
            _choiceStateMapperMock.Setup(c => c.From(ChoiceState.Yes)).Returns("yes");
            _choiceStateMapperMock.Setup(c => c.From(ChoiceState.No)).Returns("no");
            _choiceStateMapperMock.Setup(c => c.From(ChoiceState.NotStated)).Returns("not-stated");
            
            _organDonationDonationWishesMapper = fixture.Create<OrganDonationDonationWishesMapper>();
        }

        [TestMethod]
        public void MapOrganDonationWishes_WhenNotPassingDecisionDetails_ThrowsException()
        {
            // Act
            Action act = () => _organDonationDonationWishesMapper.Map(null);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(2)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("source", StringComparison.Ordinal))
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("Choices", StringComparison.Ordinal));
        }

        [TestMethod]
        public void MapOrganDonationWishes_WhenNotPassingChoices_ThrowsException()
        {
            // Act
            Action act = () => _organDonationDonationWishesMapper.Map(new DecisionDetails());

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("Choices", StringComparison.Ordinal));
        }

        [TestMethod]
        public void MapOrganDonationGender_WhenPassingDecisionDetails_MapsCorrectly()
        {
            // Act
            var result = _organDonationDonationWishesMapper.Map(new DecisionDetails
            {
                All = true,
                Choices = new Dictionary<string, ChoiceState>()
                {
                    { "heart", ChoiceState.Yes },
                    { "liver", ChoiceState.No },
                    { "tissue", ChoiceState.NotStated }
                }
            });

            // Assert
            _choiceStateMapperMock.Verify(c => c.From(ChoiceState.Yes));
            _choiceStateMapperMock.Verify(c => c.From(ChoiceState.No));
            _choiceStateMapperMock.Verify(c => c.From(ChoiceState.NotStated));
            
            result.Should().NotBeNull().And.HaveCount(4);
            result.Should().ContainKey("all").WhichValue.Should().Be("yes");
            result.Should().ContainKey("heart").WhichValue.Should().Be("yes");
            result.Should().ContainKey("liver").WhichValue.Should().Be("no");
            result.Should().ContainKey("tissue").WhichValue.Should().Be("not-stated");
        }
    }
}