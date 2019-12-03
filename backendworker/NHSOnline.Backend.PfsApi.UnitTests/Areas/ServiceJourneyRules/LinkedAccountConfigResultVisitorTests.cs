using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.PfsApi.Areas.ServiceJourneyRules;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.ServiceJourneyRules
{
    [TestClass]
    public class LinkedAccountConfigResultVisitorTests
    {
        private IFixture _fixture;
        private LinkedAccountsConfigResultVisitor _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _systemUnderTest = new LinkedAccountsConfigResultVisitor();
        }

        [TestMethod]
        public void Visit_SuccessfulWithProxyDisabled_DoesNotReturnValidLinkedAccounts()
        {
            // Arrange
            var id = Guid.NewGuid();
            var settings = new SessionConfigurationSettings(false);
            var linkedAccountsBreakdown = _fixture.Create<LinkedAccountsBreakdownSummary>();

            var successfulResult =
                new LinkedAccountsConfigResult.Success(id, settings, linkedAccountsBreakdown);

            // Act
            var result = _systemUnderTest.Visit(successfulResult);

            // Assert
            var okObjectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            var response = okObjectResult.Value.Should().BeAssignableTo<LinkedAccountsConfigResponse>().Subject;
            response.Id.Should().Be(id);
            response.HasLinkedAccounts.Should().BeFalse();
            response.LinkedAccounts.Should().BeEmpty();
        }

        [TestMethod]
        public void Visit_SuccessfulWithProxyEnabled_ReturnsValidLinkedAccountsOnly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var settings = new SessionConfigurationSettings(true);
            var linkedAccountsBreakdown = _fixture.Create<LinkedAccountsBreakdownSummary>();

            var successfulResult =
                new LinkedAccountsConfigResult.Success(id, settings, linkedAccountsBreakdown);

            // Act
            var result = _systemUnderTest.Visit(successfulResult);

            // Assert
            var okObjectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            var response = okObjectResult.Value.Should().BeAssignableTo<LinkedAccountsConfigResponse>().Subject;
            response.Id.Should().Be(id);
            response.HasLinkedAccounts.Should().BeTrue();
            response.LinkedAccounts.Should().Equal(linkedAccountsBreakdown.ValidAccounts);
        }
    }
}