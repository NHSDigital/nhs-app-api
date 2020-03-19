using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.PatientRecord;
using Moq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class TppPatientRecordServiceTests
    {
        private TppPatientRecordService _systemUnderTest;

        private GpUserSession _gpUserSession;
        private IFixture _fixture;
        private Mock<IGetPatientDcrEventsTaskChecker> _patientDcrEventsChecker;
        private Mock<IGetPatientOverviewTaskChecker> _patientOverviewTaskChecker;
        private Mock<IGetPatientTestResultsTaskChecker> _patientTestResultsChecker;
        private Mock<IGetPatientDocumentTaskChecker> _patientDocumentTaskChecker;
        private Mock<ITppClientRequest<TppUserSession, ViewPatientOverviewReply>> _patientOverview;
        private Mock<ITppClientRequest<(TppUserSession, string), RequestBinaryDataReply>> _requestBinary;
        private Mock<ITppClientRequest<TppUserSession, RequestPatientRecordReply>> _requestPatientRecord;
        private Mock<ITppClientRequest<(TppUserSession tppUserSession, string startDate, string endDate), TestResultsViewReply>> _testResultsView;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _gpUserSession = _fixture.Create<TppUserSession>();
            _patientOverview = _fixture.Freeze<Mock<ITppClientRequest<TppUserSession, ViewPatientOverviewReply>>>();
            _requestBinary = _fixture.Freeze<Mock<ITppClientRequest<(TppUserSession, string), RequestBinaryDataReply>>>();
            _requestPatientRecord = _fixture.Freeze<Mock<ITppClientRequest<TppUserSession, RequestPatientRecordReply>>>();
            _testResultsView = _fixture.Freeze<Mock<ITppClientRequest<(TppUserSession tppUserSession, string startDate, string endDate), TestResultsViewReply>>>();
            _patientDcrEventsChecker = _fixture.Freeze<Mock<IGetPatientDcrEventsTaskChecker>>();
            _patientOverviewTaskChecker = _fixture.Freeze<Mock<IGetPatientOverviewTaskChecker>>();
            _patientTestResultsChecker = _fixture.Freeze<Mock<IGetPatientTestResultsTaskChecker>>();
            _patientDocumentTaskChecker = _fixture.Freeze<Mock<IGetPatientDocumentTaskChecker>>();
            _systemUnderTest = _fixture.Create<TppPatientRecordService>();

        }

        [TestMethod]
        public async Task GetMyRecord_ReturnsSuccessResponseForHappyPath_WhenSuccessfulResponseFromTpp()
        {
            // Arrange
            var tppAllergies = CreateListPatientOverviewItem(1);
            var tppDrugSensitivities = CreateListPatientOverviewItem(2);
            var tppAcuteMedications = CreateListPatientOverviewItem(3);
            var tppCurrentRepeatMedications = CreateListPatientOverviewItem(4);
            var tppPastRepeatMedications = CreateListPatientOverviewItem(5);

            var patientOverviewResponse = new ViewPatientOverviewReply
            {
                Allergies = tppAllergies,
                DrugSensitivities = tppDrugSensitivities,
                Drugs = tppAcuteMedications,
                CurrentRepeats = tppCurrentRepeatMedications,
                PastRepeats = tppPastRepeatMedications,
            };

            var patientRecordResponse = new RequestPatientRecordReply
            {
                Events = new List<Event>
                {
                    CreateEvent()
                }
            };

            var testResultsResponse = new TestResultsViewReply
            {
                Items = new List<TestResultsViewReplyItem>
                {
                    CreateTestResultsViewReplyItem()
                }
            };

            _patientOverview.Setup(x => x.Post(It.IsAny<TppUserSession>()))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<ViewPatientOverviewReply>(HttpStatusCode.OK)
                    {
                        Body = patientOverviewResponse,
                        ErrorResponse = null,
                    }));

            _requestPatientRecord.Setup(x => x.Post(It.IsAny<TppUserSession>()))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<RequestPatientRecordReply>(HttpStatusCode.OK)
                    {
                        Body = patientRecordResponse,
                        ErrorResponse = null,
                    }));

            _testResultsView.Setup(x => x.Post(It.IsAny<(TppUserSession, string, string)>()))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<TestResultsViewReply>(HttpStatusCode.OK)
                    {
                        Body = testResultsResponse,
                        ErrorResponse = null,
                    }));

            _patientDcrEventsChecker.Setup(x =>
                    x.Check(It.IsAny<TppApiObjectResponse<RequestPatientRecordReply>>()))
                .Returns(Mock.Of<TppDcrEvents>())
                .Verifiable();

            _patientOverviewTaskChecker.Setup(x =>
                    x.Check(It.IsAny<TppApiObjectResponse<ViewPatientOverviewReply>>()))
                .Returns(new Tuple<Allergies, Medications>(Mock.Of<Allergies>(), Mock.Of<Medications>()))
                .Verifiable();

            _patientTestResultsChecker.Setup(x =>
                    x.Check(It.IsAny<TppApiObjectResponse<TestResultsViewReply>>()))
                .Returns(Mock.Of<TestResults>())
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMyRecord(new GpLinkedAccountModel(_gpUserSession));

            // Assert
            _patientOverview.Verify(x => x.Post(It.IsAny<TppUserSession>()));
            _patientDcrEventsChecker.Verify();
            _patientOverviewTaskChecker.Verify();
            _patientTestResultsChecker.Verify();

            result.Should().BeAssignableTo<GetMyRecordResult.Success>()
                .Subject.Response.Should().NotBeNull();
        }

       [TestMethod]
        public async Task GetPatientDocument_ReturnsSuccessResponse_WhenFileTooLargeErrorResponseFromTpp()
        {

            // Arrange
            var tppUserSession = new TppUserSession
            {
                PatientId = "1234",
                OnlineUserId = "12345",
                OdsCode = "1234"
            };

            var expectedErrorResponse = new Error
            {
                ErrorCode = "24",
                TechnicalMessage = "File size exceeds 2MB limit"
            };

            var patientDocument = new PatientDocument
            {
                Content = null,
                HasErrored = true,
                Type = "Document",
                IsTooLarge = true
            };

            var parameters = (tppUserSession, "test");
            _requestBinary.Setup(x => x.Post(parameters))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<RequestBinaryDataReply>(HttpStatusCode.OK)
                {
                    Body = null,
                    ErrorResponse = expectedErrorResponse
                }))
                .Verifiable();

            _patientDocumentTaskChecker.Setup(x =>
                    x.Check(It.IsAny<TppApiObjectResponse<RequestBinaryDataReply>>()))
                .Returns(patientDocument)
                .Verifiable();

            //Act
            var result = await _systemUnderTest.GetPatientDocument(tppUserSession,
                "test", null, null);


            //Assert
            _patientTestResultsChecker.Verify();

            result.Should().BeAssignableTo<GetPatientDocumentResult.Success>()
                .Subject.Response.Should().NotBeNull();
            result.Should().BeAssignableTo<GetPatientDocumentResult.Success>()
                .Subject.Response.IsTooLarge.Should().BeTrue();
            result.Should().BeAssignableTo<GetPatientDocumentResult.Success>()
                .Subject.Response.IsFileUploading.Should().BeFalse();
            result.Should().BeAssignableTo<GetPatientDocumentResult.Success>()
                .Subject.Response.HasErrored.Should().BeTrue();
            result.Should().BeAssignableTo<GetPatientDocumentResult.Success>()
                .Subject.Response.Content.Should().BeNull();
        }

        [TestMethod]
        public async Task GetPatientDocument_ReturnsSuccessResponse_WhenFileStillUploadingErrorResponseFromTpp()
        {

            // Arrange
            var tppUserSession = new TppUserSession
            {
                PatientId = "1234",
                OnlineUserId = "12345",
                OdsCode = "1234"
            };

            var expectedErrorResponse = new Error
            {
                ErrorCode = "45",
                TechnicalMessage = "File still uploading"
            };

            var patientDocument = new PatientDocument
            {
                Content = null,
                HasErrored = true,
                Type = "Document",
                IsFileUploading = true
            };

            var parameters = (tppUserSession, "test");
            _requestBinary.Setup(x => x.Post(parameters))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<RequestBinaryDataReply>(HttpStatusCode.OK)
                {
                    Body = null,
                    ErrorResponse = expectedErrorResponse
                }))
                .Verifiable();

            _patientDocumentTaskChecker.Setup(x =>
                    x.Check(It.IsAny<TppApiObjectResponse<RequestBinaryDataReply>>()))
                .Returns(patientDocument)
                .Verifiable();

            //Act
            var result = await _systemUnderTest.GetPatientDocument(tppUserSession,
                "test", null, null);


            //Assert
            _patientTestResultsChecker.Verify();

            result.Should().BeAssignableTo<GetPatientDocumentResult.Success>()
                .Subject.Response.Should().NotBeNull();
            result.Should().BeAssignableTo<GetPatientDocumentResult.Success>()
                .Subject.Response.IsTooLarge.Should().BeFalse();
            result.Should().BeAssignableTo<GetPatientDocumentResult.Success>()
                .Subject.Response.IsFileUploading.Should().BeTrue();
            result.Should().BeAssignableTo<GetPatientDocumentResult.Success>()
                .Subject.Response.HasErrored.Should().BeTrue();
            result.Should().BeAssignableTo<GetPatientDocumentResult.Success>()
                .Subject.Response.Content.Should().BeNull();
        }

        [TestMethod]
        public async Task GetPatientDocument_ReturnsSuccessResponseForHappyPath_WhenSuccessfulResponseFromTpp()
        {
            // Arrange
            const string identifier = "test";

            var tppUserSession = new TppUserSession
            {
                PatientId = "1234",
                OnlineUserId = "12345",
                OdsCode = "1234"
            };

            var expectedBinaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = "jpg",
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test",
                    }
                }
            };

            var patientDocument = new PatientDocument
            {
                Content = "test",
                HasErrored = false,
                Type = "Document",
                IsTooLarge = false
            };

            var parameters = (tppUserSession, "test");
            _requestBinary.Setup(x => x.Post(parameters))
                .Returns(Task.FromResult(new TppApiObjectResponse<RequestBinaryDataReply>(HttpStatusCode.OK)
                {
                    Body = expectedBinaryRequestResponse
                }))
                .Verifiable();

            _patientDocumentTaskChecker.Setup(x =>
                    x.Check(It.IsAny<TppApiObjectResponse<RequestBinaryDataReply>>()))
                .Returns(patientDocument)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetPatientDocument(tppUserSession, identifier,
                null, null);

            // Assert
            result.Should().BeAssignableTo<GetPatientDocumentResult.Success>()
                .Subject.Response.Should().NotBeNull();
            result.Should().BeAssignableTo<GetPatientDocumentResult.Success>()
                .Subject.Response.Content.Should().NotBeNull();
        }

        private List<ViewPatientOverViewItem> CreateListPatientOverviewItem(int count)
        {
            var result = new List<ViewPatientOverViewItem>();
            for (var i = 0; i < count; i++)
            {
                result.Add(CreatePatientOverviewItem());
            }
            return result;
        }

        private ViewPatientOverViewItem CreatePatientOverviewItem()
        {
            return new ViewPatientOverViewItem
            {
                Date = _fixture.Create<DateTimeOffset>().ToString(CultureInfo.InvariantCulture),
                Value = _fixture.Create<string>(),
            };
        }

        private Event CreateEvent()
        {
            return new Event
            {
                DoneBy = _fixture.Create<string>(),
                Location = _fixture.Create<string>(),
                Date = _fixture.Create<DateTimeOffset>().ToString(CultureInfo.InvariantCulture),
                Items = new List<RequestPatientRecordItem>
                {
                    new RequestPatientRecordItem
                    {
                        Details =_fixture.Create<string>(),
                        Type = _fixture.Create<string>()
                    }
                }
            };
        }

        private TestResultsViewReplyItem CreateTestResultsViewReplyItem()
        {
            return new TestResultsViewReplyItem
            {
                Date = _fixture.Create<DateTimeOffset>().ToString(CultureInfo.InvariantCulture),
                Value = _fixture.Create<string>(),
                Description = _fixture.Create<string>()
            };
        }
    }
}