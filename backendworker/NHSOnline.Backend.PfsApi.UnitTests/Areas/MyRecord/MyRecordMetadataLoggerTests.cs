using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.PfsApi.Areas.MyRecord;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.MyRecord
{
    [TestClass]
    public class MyRecordMetadataLoggerTests
    {
        private static IFixture _fixture;
        private Mock<ILogger<MyRecordMetadataLogger>> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<Mock<ILogger<MyRecordMetadataLogger>>>();
        }
        
        [TestMethod]
        public void LogMyRecordMetadata_LogsForEmis()
        {
            // Arrange
            var userSession = _fixture.Build<UserSession>()
                .With(x => x.GpUserSession, _fixture.Create<EmisUserSession>())
                .Create();
            
            var systemUnderTest = _fixture.Create<MyRecordMetadataLogger>();
            var myRequestResponse = new MyRecordResponse();
            var getMyRecordResponse = new GetMyRecordResult.Success(myRequestResponse);
            
            // Act
            systemUnderTest.LogMyRecordMetadata(userSession, getMyRecordResponse);
            
            // Assert
            _logger.VerifyLogger(LogLevel.Information, "medical_record_metadata=", Times.Once());
        }
        
        [TestMethod]
        public void LogMyRecordMetadata_LogsForTpp()
        {
            // Arrange
            var userSession = _fixture.Build<UserSession>()
                .With(x => x.GpUserSession, _fixture.Create<EmisUserSession>())
                .Create();
            
            var systemUnderTest = _fixture.Create<MyRecordMetadataLogger>();
            var myRequestResponse = new MyRecordResponse();
            var getMyRecordResponse = new GetMyRecordResult.Success(myRequestResponse);
            
            // Act
            systemUnderTest.LogMyRecordMetadata(userSession, getMyRecordResponse);
            
            // Assert
            _logger.VerifyLogger(LogLevel.Information,"medical_record_metadata=", Times.Once());
        }
        
        [TestMethod]
        public void LogMyRecordMetadata_DoesNotLogForVision()
        {
            // Arrange
            var userSession = _fixture.Build<UserSession>()
                .With(x => x.GpUserSession, _fixture.Create<VisionUserSession>())
                .Create();
            
            var systemUnderTest = _fixture.Create<MyRecordMetadataLogger>();
            var myRequestResponse = new MyRecordResponse();
            var getMyRecordResponse = new GetMyRecordResult.Success(myRequestResponse);
            
            // Act
            systemUnderTest.LogMyRecordMetadata(userSession, getMyRecordResponse);
            
            // Assert
            _logger.VerifyLogger(LogLevel.Information, "medical_record_metadata=", Times.Never());
        }
        
        [TestMethod]
        public void LogMyRecordMetadata_DoesNotLogForMicrotest()
        {
            // Arrange
            var userSession = _fixture.Build<UserSession>()
                .With(x => x.GpUserSession, _fixture.Create<MicrotestUserSession>())
                .Create();
            
            var systemUnderTest = _fixture.Create<MyRecordMetadataLogger>();
            var myRequestResponse = new MyRecordResponse();
            var getMyRecordResponse = new GetMyRecordResult.Success(myRequestResponse);
            
            // Act
            systemUnderTest.LogMyRecordMetadata(userSession, getMyRecordResponse);
            
            // Assert
            _logger.VerifyLogger(LogLevel.Information, "medical_record_metadata=", Times.Never());
        }
        
        [TestMethod]
        public void LogEmisMyRecordMetadata_TppDcrEventsAreNull()
        {
            // Arrange
            var myRequestResponse = new MyRecordResponse();
            var getMyRecordResponse = new GetMyRecordResult.Success(myRequestResponse);
            var userSession = _fixture.Build<UserSession>()
                .With(x => x.GpUserSession, _fixture.Create<EmisUserSession>())
                .Create();

            // Act
            var result = MyRecordMetadataLogger.BuildMyRecordMetadata(userSession, getMyRecordResponse);
            
            // Assert
            FindRecordDetailMetadata(result, "Tpp Dcr Events").HasAccess.Should().BeNull();
        }
        
        [DataTestMethod]
        [DataRow(2, 3, 1, 5, 5, 0, 3)]
        [DataRow(5, 2, 3, 8, 7, 0, 9)]
        public void LogEmisMyRecordMetadata_WithVaryingAmountsOfDataItems(
            int allergyCount,
            int medicationCount,
            int immunisationCount,
            int problemCount,
            int consultationCount,
            int tppDcrEventCount,
            int testResultCount
        )
        {
            // Arrange
            var myRequestResponse = BuildPopulatedMyRecordResponse(
                allergyCount, medicationCount,immunisationCount,
                problemCount,tppDcrEventCount,consultationCount, testResultCount);
            var getMyRecordResponse = new GetMyRecordResult.Success(myRequestResponse);
            var userSession = _fixture.Build<UserSession>()
                .With(x => x.GpUserSession, _fixture.Create<EmisUserSession>())
                .Create();

            // Act
            var result = MyRecordMetadataLogger.BuildMyRecordMetadata(userSession, getMyRecordResponse);
            
            // Assert
            var allergyData = FindRecordDetailMetadata(result, "Allergies");
            allergyData.ItemCount.Should().Be(allergyCount);
            allergyData.HasAccess.Should().Be(myRequestResponse.Allergies.HasAccess);
                
            var medicationsData = FindRecordDetailMetadata(result, "Medications");
            medicationsData.ItemCount.Should().Be(medicationCount);
            medicationsData.HasAccess.Should().Be(myRequestResponse.Medications.HasAccess);
            
            var immunisationsData = FindRecordDetailMetadata(result, "Immunisations");
            immunisationsData.ItemCount.Should().Be(immunisationCount);
            immunisationsData.HasAccess.Should().Be(myRequestResponse.Immunisations.HasAccess);
            
            var problemsData = FindRecordDetailMetadata(result, "Problems");
            problemsData.ItemCount.Should().Be(problemCount);
            problemsData.HasAccess.Should().Be(myRequestResponse.Problems.HasAccess);
            
            var consultationsData = FindRecordDetailMetadata(result, "Consultations");
            consultationsData.ItemCount.Should().Be(consultationCount);
            consultationsData.HasAccess.Should().Be(myRequestResponse.Consultations.HasAccess);
            
            var tppDcrEventsData = FindRecordDetailMetadata(result, "TPP DCR Events");
            tppDcrEventsData.ItemCount.Should().Be(tppDcrEventCount);
            tppDcrEventsData.HasAccess.Should().Be(null);
            
            var testResultsData = FindRecordDetailMetadata(result, "Test Results");
            testResultsData.ItemCount.Should().Be(testResultCount);
            testResultsData.HasAccess.Should().Be(myRequestResponse.TestResults.HasAccess);
        }
        
        [TestMethod]
        public void LogTppMyRecordMetadata_EmisRecordDetailMetadataitemsAreNull()
        {
            // Arrange
            var myRequestResponse = new MyRecordResponse();
            var getMyRecordResponse = new GetMyRecordResult.Success(myRequestResponse);
            var userSession = _fixture.Build<UserSession>()
                .With(x => x.GpUserSession, _fixture.Create<TppUserSession>())
                .Create();

            // Act
            var result = MyRecordMetadataLogger.BuildMyRecordMetadata(userSession, getMyRecordResponse);
            
            // Assert
            FindRecordDetailMetadata(result, "Immunisations").HasAccess.Should().BeNull();
            FindRecordDetailMetadata(result, "Problems").HasAccess.Should().BeNull();
            FindRecordDetailMetadata(result, "Consultations").HasAccess.Should().BeNull();
        }
        
        [DataTestMethod]
        [DataRow(2, 3, 0, 0, 0, 5, 3)]
        [DataRow(5, 2, 0, 0, 0, 7, 9)]
        public void LogTppMyRecordMetadata_WithVaryingAmountsOfDataItems(
            int allergyCount,
            int medicationCount,
            int immunisationCount,
            int problemCount,
            int consultationCount,
            int tppDcrEventCount,
            int testResultCount
            )
        {
            // Arrange
            var myRequestResponse = BuildPopulatedMyRecordResponse(
                allergyCount, medicationCount,immunisationCount,
                problemCount,tppDcrEventCount,consultationCount, testResultCount);
            var getMyRecordResponse = new GetMyRecordResult.Success(myRequestResponse);
            var userSession = _fixture.Build<UserSession>()
                .With(x => x.GpUserSession, _fixture.Create<TppUserSession>())
                .Create();

            // Act
            var result = MyRecordMetadataLogger.BuildMyRecordMetadata(userSession, getMyRecordResponse);
            
            // Assert
            var allergyData = FindRecordDetailMetadata(result, "Allergies");
            allergyData.ItemCount.Should().Be(allergyCount);
            allergyData.HasAccess.Should().Be(myRequestResponse.Allergies.HasAccess);
                
            var medicationsData = FindRecordDetailMetadata(result, "Medications");
            medicationsData.ItemCount.Should().Be(medicationCount);
            medicationsData.HasAccess.Should().Be(myRequestResponse.Medications.HasAccess);
            
            var immunisationsData = FindRecordDetailMetadata(result, "Immunisations");
            immunisationsData.ItemCount.Should().Be(immunisationCount);
            immunisationsData.HasAccess.Should().BeNull();
            
            var problemsData = FindRecordDetailMetadata(result, "Problems");
            problemsData.ItemCount.Should().Be(problemCount);
            problemsData.HasAccess.Should().BeNull();
            
            var consultationsData = FindRecordDetailMetadata(result, "Consultations");
            consultationsData.ItemCount.Should().Be(consultationCount);
            consultationsData.HasAccess.Should().BeNull();
            
            var tppDcrEventsData = FindRecordDetailMetadata(result, "TPP DCR Events");
            tppDcrEventsData.ItemCount.Should().Be(tppDcrEventCount);
            tppDcrEventsData.HasAccess.Should().Be(myRequestResponse.TppDcrEvents.HasAccess);
            
            var testResultsData = FindRecordDetailMetadata(result, "Test Results");
            testResultsData.ItemCount.Should().Be(testResultCount);
            testResultsData.HasAccess.Should().Be(myRequestResponse.TestResults.HasAccess);
        }

        private static MyRecordMetadataLogger.RecordDetailMetaData FindRecordDetailMetadata(MyRecordMetadataLogger.MedicalRecordMetadata medicalRecordMetadata, string name)
        {
            return medicalRecordMetadata.RecordDetailMetaData.Single(x =>
                x.DetailName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        
        //Response Builder to allow tests against differing data sizes
        private static MyRecordResponse BuildPopulatedMyRecordResponse(int allergyCount, int medicationCount,
            int immunisationCount, int problemCount, int tppDcrEventCount,
            int consultationCount, int testResultsCount)
        {
            return new MyRecordResponse
            {
                Allergies = GenerateAllergies(allergyCount),
                Consultations =  GenerateConsultations(consultationCount),
                Immunisations = GenerateImmunisations(immunisationCount),
                Medications = GenerateMedications(medicationCount),
                Problems = GenerateProblems(problemCount),
                TestResults = GenerateTestResults(testResultsCount),
                TppDcrEvents = GenerateTppDcrEvents(tppDcrEventCount)
            };
        }

        private static Allergies GenerateAllergies(int allergyCount)
        {
            return _fixture.Build<Allergies>()
                .With(x => x.Data, _fixture.CreateMany<AllergyItem>(allergyCount))
                .Create();
        }
        
        private static Consultations GenerateConsultations(int consultationCount)
        {
            return _fixture.Build<Consultations>()
                .With(x => x.Data, _fixture.CreateMany<ConsultationItem>(consultationCount))
                .Create();
        }
        
        private static Immunisations GenerateImmunisations(int immunisationCount)
        {  
            return _fixture.Build<Immunisations>()
                .With(x => x.Data, _fixture.CreateMany<ImmunisationItem>(immunisationCount))
                .Create();
        }
        
        private static Medications GenerateMedications(int medicationCount)
        {
            var acuteCount = medicationCount / 3;
            var currentCount = medicationCount / 3;
            var discontinuedCount = medicationCount - (acuteCount + currentCount);

            var medicationData = _fixture.Build<MedicationsData>()
                .With(x => x.AcuteMedications, _fixture.CreateMany<MedicationItem>(acuteCount))
                .With(x => x.CurrentRepeatMedications, _fixture.CreateMany<MedicationItem>(currentCount))
                .With(x => x.DiscontinuedRepeatMedications, _fixture.CreateMany<MedicationItem>(discontinuedCount))
                .Create();

            return _fixture.Build<Medications>()
                .With(x => x.Data, medicationData)
                .Create();
        }
        
        private static Problems GenerateProblems(int problemCount)
        {
            return _fixture.Build<Problems>()
                .With(x => x.Data, _fixture.CreateMany<ProblemItem>(problemCount))
                .Create();
        }
        
        private static TestResults GenerateTestResults(int testResultsCount)
        {
            return _fixture.Build<TestResults>()
                .With(x => x.Data, _fixture.CreateMany<TestResultItem>(testResultsCount))
                .Create();
        }
        
        private static TppDcrEvents GenerateTppDcrEvents(int tppDcrEventsCount)
        {  
            return _fixture.Build<TppDcrEvents>()
                .With(x => x.Data, _fixture.CreateMany<TppDcrEvent>(tppDcrEventsCount).ToList())
                .Create();
        }
    }
}