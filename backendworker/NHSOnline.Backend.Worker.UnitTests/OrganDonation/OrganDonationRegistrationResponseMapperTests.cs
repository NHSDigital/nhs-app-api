using System;
using System.Collections.Generic;
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
    public class OrganDonationRegistrationResponseMapperTests
    {
        private IFixture _fixture;

        private IMapper<OrganDonationResponse<RegistrationResponse>, OrganDonationRegistrationResponse>
            _registrationResponseMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _registrationResponseMapper = _fixture.Create<OrganDonationRegistrationResponseMapper>();
        }

        [TestMethod]
        public void MapOrganDonationResponse_WhenPassingNull_ThrowsException()
        {
            // Act and assert
            Action act = () => _registrationResponseMapper.Map(null);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(2)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("source", StringComparison.Ordinal))
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("Body", StringComparison.Ordinal));
        }

        [TestMethod]
        public void MapOrganDonationResponse_WhenNotPassingBody_ThrowsException()
        {
            // Act and assert
            Action act = () =>
                _registrationResponseMapper.Map(new OrganDonationResponse<RegistrationResponse>(HttpStatusCode.OK));

            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("Body");
        }

        [TestMethod]
        public void MapOrganDonationResponse_WithAllValues_MapsTheIdentifier()
        {
            // Arrange
            var response = new OrganDonationResponse<RegistrationResponse>(HttpStatusCode.OK)
            {
                Body = _fixture.Create<RegistrationResponse>()
            };
            
            // Act
            var result = _registrationResponseMapper.Map(response);
            
            // Assert
            result.Should().NotBeNull();
            result.Identifier.Should().Be(response.Body.Id);
            result.State.Should().Be(State.Ok);
        }

        [TestMethod]
        [DataRow("10001")]
        [DataRow("10002")]
        [DataRow("10201")]
        [DataRow("10202")]
        public void MapOrganDonationResponse_WithConflictErrorCode_SetsIsConflictedFlag(string code)
        {
            var body = _fixture.Create<RegistrationResponse>();
            body.Issue = CreateIssueWithErrorCode(code);

            // Arrange
            var response = new OrganDonationResponse<RegistrationResponse>(HttpStatusCode.OK)
            {
                Body = body
            };

            // Act
            var result = _registrationResponseMapper.Map(response);

            // Assert
            result.Should().NotBeNull();
            result.Identifier.Should().Be(response.Body.Id);
            result.State.Should().Be(State.Conflicted);
        }

        private static List<Issue> CreateIssueWithErrorCode(string errorCode)
        {
            return new List<Issue>
            {
                new Issue
                {
                    Details = new CodeableConcept
                    {
                        Coding = new List<Coding> { new Coding { Code = errorCode } }
                    }
                }
            };
        }
    }
}