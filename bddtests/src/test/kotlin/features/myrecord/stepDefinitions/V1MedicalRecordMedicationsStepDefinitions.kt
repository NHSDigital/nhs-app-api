package features.myrecord.stepDefinitions

import constants.DateTimeFormats
import constants.ErrorResponseCodeTpp
import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.MedicationsFactory
import mocking.tpp.models.Error
import net.serenitybdd.core.Serenity
import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.assertIsVisible
import pages.isVisible
import pages.myrecord.MedicalRecordV1Page
import pages.myrecord.RecordItem
import utils.SerenityHelpers
import worker.models.myrecord.MedicationItem
import worker.models.myrecord.MyRecordResponse
import java.time.LocalDate
import java.time.format.DateTimeFormatter
import mocking.data.myrecord.MedicationsData
import java.lang.UnsupportedOperationException

open class V1MedicalRecordMedicationsStepDefinitions : AbstractDemographicsStepDefinitions() {

    private lateinit var medicalRecordV1Page: MedicalRecordV1Page

    @Given("^the GP Practice has enabled medications functionality$")
    fun givenTheGPPracticeHasEnabledMedicationsFunctionality() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        MedicationsFactory.getForSupplier(gpSystem).enabledWithRecords(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has disabled medications functionality$")
    fun butTheGPPracticeHasDisabledMedicationsFunctionality() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatient()

        when (gpSystem) {
            Supplier.EMIS -> {
                mockingClient.forEmis {
                    myRecord.medicationsRequest(patient)
                            .respondWithExceptionWhenNotEnabled()
                }
            }
            Supplier.TPP -> {
                mockingClient.forTpp {
                    myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                            .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                                    "Requested record access is disabled by the practice",
                                    "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                }
            }
            else -> throw UnsupportedOperationException("Not implemented for $gpSystem")
        }
    }

    @Given("^the EMIS GP Practice has acute medication results where the first record has no date$")
    fun theEmisGpPracticeHasAcuteMedicationResultsWhereTheFirstRecordHasNoDate() {
        mockingClient.forEmis {
            myRecord.medicationsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(MedicationsData.getEmisAcuteMedicationsResponseWhereTheFirstResultHasNoDate())
        }
    }

    @Then("^I see the expected acute medications displayed without the record with an unknown date" +
            " - Medical Record v1$")
    fun thenISeeTheExpectedAcuteMedicationsDisplayedWithoutTheRecordWithAnUnknownDateV1() {
        val expectedAcuteMedications = MedicationsData.
                getEmisAcuteMedicationsResponseWhereTheFirstResultHasNoDate().medicalRecord.medication

        val onScreenAcuteMedications = medicalRecordV1Page.acuteMedications.allRecordItems()

        assertEquals(expectedAcuteMedications.size, onScreenAcuteMedications.count())

        for (i in onScreenAcuteMedications.indices){
            if(i == 0){
                assertEquals("Unknown Date", onScreenAcuteMedications[i].label)
            } else {
                val expectedDate = expectedAcuteMedications[i].firstIssueDate?.takeWhile { !it.isLetter() }
                val actualDate = LocalDate.parse(onScreenAcuteMedications[i].label,
                        DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)).toString()

                assertEquals(expectedDate, actualDate)
            }
        }
    }

    @Given("^the EMIS GP Practice has current repeat medication results where the first record has no date$")
    fun theEmisGpPracticeHasCurrentRepeatMedicationResultsWhereTheFirstRecordHasNoDate() {
        mockingClient.forEmis {
            myRecord.medicationsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(MedicationsData.
                            getEmisCurrentRepeatMedicationsResponseWhereTheFirstResultHasNoDate())
        }
    }

    @Then("^I see the expected current repeat medications displayed with the first record with unknown date" +
            " - Medical Record v1$")
    fun thenISeeTheExpectedCurrentRepeatMedicationsDisplayedWithTheFirstRecordWithUnknownDateV1() {
        val expectedCurrentRepeatMedications = MedicationsData.
                getEmisCurrentRepeatMedicationsResponseWhereTheFirstResultHasNoDate().medicalRecord.medication

        val onScreenCurrentRepeatMedications = medicalRecordV1Page.repeatMedications.allRecordItems()

        assertEquals(expectedCurrentRepeatMedications.size, onScreenCurrentRepeatMedications.count())

        for (i in onScreenCurrentRepeatMedications.indices){
            if(i == 0){
                assertEquals("Unknown Date", onScreenCurrentRepeatMedications[i].label)
            } else {
                val expectedDate = expectedCurrentRepeatMedications[i].firstIssueDate?.takeWhile { !it.isLetter() }
                val actualDate = LocalDate.parse(onScreenCurrentRepeatMedications[i].label,
                        DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)).toString()

                assertEquals(expectedDate, actualDate)
            }
        }
    }

    @Given("^the EMIS GP Practice has discontinued repeat medication results where the first record has no date$")
    fun theEmisGpPracticeHasDiscontinuedRepeatMedicationResultsWhereTheFirstRecordHasNoDate() {
        mockingClient.forEmis {
            myRecord.medicationsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(MedicationsData.
                            getEmisDiscontinuedRepeatMedicationsResponseWhereTheFirstResultHasNoDate())
        }
    }

    @Then("^I see the expected discontinued repeat medications displayed with the first record with unknown" +
            " date - Medical Record v1$")
    fun thenISeeTheExpectedDiscontinuedRepeatMedicationsDisplayedWithTheFirstRecordWithUnknownDateV1() {
        val expectedDiscontinuedRepeatMedications = MedicationsData.
                getEmisDiscontinuedRepeatMedicationsResponseWhereTheFirstResultHasNoDate().medicalRecord.medication
        val onScreenDiscontinuedRepeatMedications = medicalRecordV1Page.discontinuedRepeatMedications.allRecordItems()
        assertEquals(expectedDiscontinuedRepeatMedications.size, onScreenDiscontinuedRepeatMedications.count())
        for (i in onScreenDiscontinuedRepeatMedications.indices){
            if(i == 0){
                assertEquals("Unknown Date", onScreenDiscontinuedRepeatMedications[i].label)
            } else {
                val expectedDate = expectedDiscontinuedRepeatMedications[i].firstIssueDate?.takeWhile { !it.isLetter()}
                val actualDate = LocalDate.parse(onScreenDiscontinuedRepeatMedications[i].label,
                        DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)).toString()
                assertEquals(expectedDate, actualDate)
            }
        }
    }

    @When("^the flag informing that the patient has access to the medications data is set to \"(.*)\"$")
    fun andHasAccessToMedicationsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.medications.hasAccess)
    }

    @When("^the flag informing that there was an error retrieving the medications data is set to \"(.*)\"$")
    fun andHasErrorsWhenRetrievingMedicationsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.medications.hasErrored)
    }

    @Then("^I receive \"(.*)\" acute medications as part of the my record object$")
    fun thenIReceiveAnAcuteMedicationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.medications.data.acuteMedications.count())
    }

    @Then("^I receive \"(.*)\" current repeat medications as part of the my record object$")
    fun thenIReceiveACurrentRepeatMedicationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.medications.data.currentRepeatMedications.count())
    }

    @Then("^I receive \"(.*)\" discontinued repeat medications as part of the my record object$")
    fun thenIReceiveADiscontinuedRepeatMedicationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.medications.data.discontinuedRepeatMedications.count())
    }

    @Then("^I see current repeat medication information - Medical Record v1$")
    fun thenISeeCurrentRepeatMedicationInformationV1() {
        Assert.assertTrue(medicalRecordV1Page.repeatMedications.firstElement.isVisible)
    }

    @Then("^I see discontinued repeat medication information - Medical Record v1$")
    fun thenISeeDiscontinuedRepeatMedicationInformationV1() {
        Assert.assertTrue(medicalRecordV1Page.discontinuedRepeatMedications.firstElement.isVisible)
    }

    @Then("^I see the expected acute medications displayed - Medical Record v1$")
    fun thenISeeTheExpectedAcuteMedicationsDisplayedV1() {
        val expectedAcuteMedications = MedicationsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .getExpectedMedications().acuteMedications

        val onScreenAcuteMedications = medicalRecordV1Page.acuteMedications.allRecordItems()

        checkOnScreenMedicationsAreCorrect(expectedAcuteMedications, onScreenAcuteMedications)
    }

    @Then("^I see the expected discontinued repeat medications displayed - Medical Record v1$")
    fun thenISeeTheExpectedDiscontinuedRepeatMedicationsDisplayedV1() {
        val expectedHistoricMedications = MedicationsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .getExpectedMedications().discontinuedRepeatMedications

        val onScreenHistoricMedications = medicalRecordV1Page.discontinuedRepeatMedications.allRecordItems()

        checkOnScreenMedicationsAreCorrect(expectedHistoricMedications, onScreenHistoricMedications)
    }

    @Then("^I see the expected current repeat medications displayed - Medical Record v1$")
    fun thenISeeTheExpectedCurrentRepeatMedicationsDisplayedV1() {
        val expectedCurrentMedications = MedicationsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .getExpectedMedications().currentRepeatMedications

        val onScreenCurrentMedications = medicalRecordV1Page.repeatMedications.allRecordItems()

        checkOnScreenMedicationsAreCorrect(expectedCurrentMedications, onScreenCurrentMedications)
    }

    @Then("^I see acute medication information - Medical Record v1$")
    fun thenISeeAcuteMedicationInformationV1() {
        medicalRecordV1Page.acuteMedications.firstElement.assertIsVisible()
    }

    private fun checkOnScreenMedicationsAreCorrect(expectedMedications: List<MedicationItem>,
                                                   actualMedications: List<RecordItem>) {
        assertEquals(expectedMedications.count(), actualMedications.count())

        for (i in actualMedications.indices) {
            assertEquals(
                    LocalDate.parse(expectedMedications[i].date),
                    LocalDate.parse(actualMedications[i].label,
                            DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)
                    )
            )

            for (j in expectedMedications[i].lineItems.indices) {
                assertEquals(expectedMedications[i].lineItems[j].text, actualMedications[i].bodyElements[j])
            }
        }
    }
}