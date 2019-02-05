using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord;
using Microsoft.Extensions.Logging;
using Moq;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class TppPatientOverviewMapperTests
    {
        private IFixture _fixture;
        private ITppMyRecordMapper _mapper;
        private ILogger<TppPatientOverviewMapper> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new TppMyRecordMapper();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = Mock.Of<ILogger<TppPatientOverviewMapper>>();
        } 

        [TestMethod]
        public void MapAllergyRequestsGetResponseToAllergyListResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new ViewPatientOverviewReply();

            // Act
            var patientOverview = new TppPatientOverviewMapper(_logger).Map(item);
            var result = _mapper.Map(patientOverview.Item1, patientOverview.Item2, new TppDcrEvents(), new TestResults());

            // Assert
            result.Should().NotBeNull();
            result.Allergies.Data.Should().BeEmpty();
            result.Medications.Data.AcuteMedications.Should().BeEmpty();
            result.Medications.Data.CurrentRepeatMedications.Should().BeEmpty();
            result.Medications.Data.DiscontinuedRepeatMedications.Should().BeEmpty();
        }

        [TestMethod]
        public void MapAllergyRequestsGetResponseToAllergyListResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var tppAllergies = CreateListPatientOverviewItem(1);
            var tppDrugSensitivities = CreateListPatientOverviewItem(2);
            var tppAcuteMedications = CreateListPatientOverviewItem(3);
            var tppCurrentRepeatMedications = CreateListPatientOverviewItem(4);
            var tppPastRepeatMedications = CreateListPatientOverviewItem(5);
            
            var patientOverview = new ViewPatientOverviewReply
            {
                Allergies = tppAllergies,
                DrugSensitivities = tppDrugSensitivities,
                Drugs = tppAcuteMedications,
                CurrentRepeats = tppCurrentRepeatMedications,
                PastRepeats = tppPastRepeatMedications,
            };
                    
            var expectedAllegies = CreateListAllergyItem(patientOverview.Allergies);
            expectedAllegies.AddRange(CreateListAllergyItem(patientOverview.DrugSensitivities));
            var expectedAcuteMedications = CreateListMedicationItem(patientOverview.Drugs);
            var expectedCurrentRepeatMedications = CreateListMedicationItem(patientOverview.CurrentRepeats);
            var expectedPastRepeatMedications = CreateListMedicationItem(patientOverview.PastRepeats);
            
            // Act
            var result = new TppPatientOverviewMapper(_logger).Map(patientOverview);
            var mappedAllergies = result.Item1;
            var mappedMedications= result.Item2;
            
            // Assert
            result.Should().NotBeNull();
            mappedAllergies.Data.Should().HaveCount(patientOverview.Allergies.Count + patientOverview.DrugSensitivities.Count);
            mappedAllergies.Data.Should().BeEquivalentTo(expectedAllegies);
            mappedMedications.Data.AcuteMedications.Should().HaveCount(patientOverview.Drugs.Count);
            mappedMedications.Data.AcuteMedications.Should().BeEquivalentTo(expectedAcuteMedications);
            mappedMedications.Data.CurrentRepeatMedications.Should().HaveCount(patientOverview.CurrentRepeats.Count);
            mappedMedications.Data.CurrentRepeatMedications.Should().BeEquivalentTo(expectedCurrentRepeatMedications);
            mappedMedications.Data.DiscontinuedRepeatMedications.Should().HaveCount(patientOverview.PastRepeats.Count);
            mappedMedications.Data.DiscontinuedRepeatMedications.Should().BeEquivalentTo(expectedPastRepeatMedications);         
        }

        private List<ViewPatientOverViewItem> CreateListPatientOverviewItem(int count)
        {
            var result = new List<ViewPatientOverViewItem>();
            for (int i = 0; i < count; i++)
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

        private List<AllergyItem> CreateListAllergyItem(List<ViewPatientOverViewItem> items)
        {
            return items.Select(x => CreateAllergyItem(x)).ToList();
        }

        private List<MedicationItem> CreateListMedicationItem(List<ViewPatientOverViewItem> items)
        {
            return items.Select(x => CreateMedicationItem(x)).ToList();
        }

        private AllergyItem CreateAllergyItem(ViewPatientOverViewItem item)
        {
            return new AllergyItem
            {
                Name = item.Value,
                Date = new MyRecordDate
                {
                    Value = DateTimeOffset.Parse(item.Date, CultureInfo.InvariantCulture)
                }
            };
        }

        private MedicationItem CreateMedicationItem(ViewPatientOverViewItem item)
        {
            var result = new MedicationItem
            {
                Date = DateTimeOffset.Parse(item.Date, CultureInfo.InvariantCulture)
            };
            
            var medicationLineItems = new List<MedicationLineItem>();
            medicationLineItems.Add(new MedicationLineItem {Text = item.Value});
            result.LineItems = medicationLineItems;
            return result;
        }
    }
}
