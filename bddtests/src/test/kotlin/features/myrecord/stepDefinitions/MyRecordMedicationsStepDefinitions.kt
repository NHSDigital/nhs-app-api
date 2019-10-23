package features.myrecord.stepDefinitions

import constants.DateTimeFormats
import constants.ErrorResponseCodeTpp
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
import pages.myrecord.MyRecordInfoPage
import pages.myrecord.RecordItem
import utils.SerenityHelpers
import worker.models.myrecord.MedicationItem
import worker.models.myrecord.MyRecordResponse
import java.time.LocalDate
import java.time.format.DateTimeFormatter
import mocking.data.myrecord.MedicationsData

open class MyRecordMedicationsStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Given("^the GP Practice has enabled medications functionality$")
    fun givenTheGPPracticeHasEnabledMedicationsFunctionality() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        MedicationsFactory.getForSupplier(gpSystem).enabledWithRecords(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has enabled medication functionality and the patient has no medications$")
    fun givenTheGPPracticeHasEnabledMedicationsFunctionalityAndPatientHasNoMedications() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        val factory = MedicationsFactory.getForSupplier(gpSystem)
        factory.enabledWithBlankRecord(SerenityHelpers.getPatient())
        factory.getResult()
    }

    @Given("^the GP Practice has disabled medications functionality$")
    fun butTheGPPracticeHasDisabledMedicationsFunctionality() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        when (gpSystem) {
            "EMIS" -> {
                mockingClient.forEmis {
                    myRecord.medicationsRequest(SerenityHelpers.getPatient())
                            .respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    myRecord.viewPatientOverviewPost(SerenityHelpers.getPatient().tppUserSession!!)
                            .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                                    "Requested record access is disabled by the practice",
                                    "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                }
            }
        }
    }

    @Given("^the EMIS GP Practice has acute medication results where the first record has no date$")
    fun theEmisGpPracticeHasAcuteMedicationResultsWhereTheFirstRecordHasNoDate() {
        setPatientToDefaultFor("EMIS")
        val patient = SerenityHelpers.getPatient()

        mockingClient.forEmis {
            myRecord.medicationsRequest(patient)
                    .respondWithSuccess(MedicationsData.getEmisAcuteMedicationsResponseWhereTheFirstResultHasNoDate())
        }
    }

    @Then("^I see the expected acute medications displayed without the record with an unknown date$")
    fun thenISeeTheExpectedAcuteMedicationsDisplayedWithoutTheRecordWithAnUnknownDate() {
        val expectedAcuteMedications = MedicationsData.
                getEmisAcuteMedicationsResponseWhereTheFirstResultHasNoDate().medicalRecord.medication

        val onScreenAcuteMedications = myRecordInfoPage.acuteMedications.allRecordItems()

        Assert.assertEquals(expectedAcuteMedications.size, onScreenAcuteMedications.count())

        for (i in onScreenAcuteMedications.indices){
            if(i == 0){
                Assert.assertEquals("Unknown Date", onScreenAcuteMedications[i].label)
            } else {
                val expectedDate = expectedAcuteMedications[i].firstIssueDate?.takeWhile { !it.isLetter() }
                val actualDate = LocalDate.parse(onScreenAcuteMedications[i].label,
                        DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)).toString()

                Assert.assertEquals(expectedDate, actualDate)
            }
        }
    }

    @Given("^the EMIS GP Practice has current repeat medication results where the first record has no date$")
    fun theEmisGpPracticeHasCurrentRepeatMedicationResultsWhereTheFirstRecordHasNoDate() {
        setPatientToDefaultFor("EMIS")
        val patient = SerenityHelpers.getPatient()

        mockingClient.forEmis {
            myRecord.medicationsRequest(patient)
                    .respondWithSuccess(MedicationsData.
                            getEmisCurrentRepeatMedicationsResponseWhereTheFirstResultHasNoDate())
        }
    }

    @Then("^I see the expected current repeat medications displayed with the first record with unknown date$")
    fun thenISeeTheExpectedCurrentRepeatMedicationsDisplayedWithTheFirstRecordWithUnknownDate() {
        val expectedCurrentRepeatMedications = MedicationsData.
                getEmisCurrentRepeatMedicationsResponseWhereTheFirstResultHasNoDate().medicalRecord.medication

        val onScreenCurrentRepeatMedications = myRecordInfoPage.repeatMedications.allRecordItems()

        Assert.assertEquals(expectedCurrentRepeatMedications.size, onScreenCurrentRepeatMedications.count())

        for (i in onScreenCurrentRepeatMedications.indices){
            if(i == 0){
                Assert.assertEquals("Unknown Date", onScreenCurrentRepeatMedications[i].label)
            } else {
                val expectedDate = expectedCurrentRepeatMedications[i].firstIssueDate?.takeWhile { !it.isLetter() }
                val actualDate = LocalDate.parse(onScreenCurrentRepeatMedications[i].label,
                        DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)).toString()

                Assert.assertEquals(expectedDate, actualDate)
            }
        }
    }

    @Given("^the EMIS GP Practice has discontinued repeat medication results where the first record has no date$")
    fun theEmisGpPracticeHasDiscontinuedRepeatMedicationResultsWhereTheFirstRecordHasNoDate() {
        setPatientToDefaultFor("EMIS")
        val patient = SerenityHelpers.getPatient()
        mockingClient.forEmis {
            myRecord.medicationsRequest(patient)
                    .respondWithSuccess(MedicationsData.
                            getEmisDiscontinuedRepeatMedicationsResponseWhereTheFirstResultHasNoDate())
        }
    }

    @Then("^I see the expected discontinued repeat medications displayed with the first record with unknown date$")
    fun thenISeeTheExpectedDiscontinuedRepeatMedicationsDisplayedWithTheFirstRecordWithUnknownDate() {
        val expectedDiscontinuedRepeatMedications = MedicationsData.
                getEmisDiscontinuedRepeatMedicationsResponseWhereTheFirstResultHasNoDate().medicalRecord.medication
        val onScreenDiscontinuedRepeatMedications = myRecordInfoPage.discontinuedRepeatMedications.allRecordItems()
        Assert.assertEquals(expectedDiscontinuedRepeatMedications.size, onScreenDiscontinuedRepeatMedications.count())
        for (i in onScreenDiscontinuedRepeatMedications.indices){
            if(i == 0){
                Assert.assertEquals("Unknown Date", onScreenDiscontinuedRepeatMedications[i].label)
            } else {
                val expectedDate = expectedDiscontinuedRepeatMedications[i].firstIssueDate?.takeWhile { !it.isLetter()}
                val actualDate = LocalDate.parse(onScreenDiscontinuedRepeatMedications[i].label,
                        DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)).toString()
                Assert.assertEquals(expectedDate, actualDate)
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

    @Then("^I see current repeat medication information$")
    fun thenISeeCurrentRepeatMedicationInformation() {
        Assert.assertTrue(myRecordInfoPage.repeatMedications.firstElement.isVisible)
    }

    @Then("^I see discontinued repeat medication information$")
    fun thenISeeDiscontinuedRepeatMedicationInformation() {
        Assert.assertTrue(myRecordInfoPage.discontinuedRepeatMedications.firstElement.isVisible)
    }

    @Then("^I see the expected acute medications displayed$")
    fun thenISeeTheExpectedAcuteMedicationsDisplayed() {
        val expectedAcuteMedications = MedicationsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .getExpectedMedications().acuteMedications

        val onScreenAcuteMedications = myRecordInfoPage.acuteMedications.allRecordItems()

        checkOnScreenMedicationsAreCorrect(expectedAcuteMedications, onScreenAcuteMedications)
    }

    @Then("^I see the expected discontinued repeat medications displayed$")
    fun thenISeeTheExpectedDiscontinuedRepeatMedicationsDisplayed() {
        val expectedHistoricMedications = MedicationsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .getExpectedMedications().discontinuedRepeatMedications

        val onScreenHistoricMedications = myRecordInfoPage.discontinuedRepeatMedications.allRecordItems()

        checkOnScreenMedicationsAreCorrect(expectedHistoricMedications, onScreenHistoricMedications)
    }

    @Then("^I see the expected current repeat medications displayed$")
    fun thenISeeTheExpectedCurrentRepeatMedicationsDisplayed() {
        val expectedCurrentMedications = MedicationsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .getExpectedMedications().currentRepeatMedications

        val onScreenCurrentMedications = myRecordInfoPage.repeatMedications.allRecordItems()

        checkOnScreenMedicationsAreCorrect(expectedCurrentMedications, onScreenCurrentMedications)
    }

    @Then("^I see acute medication information$")
    fun thenISeeAcuteMedicationInformation() {
        myRecordInfoPage.acuteMedications.firstElement.assertIsVisible()
    }

    private fun checkOnScreenMedicationsAreCorrect(expectedMedications: List<MedicationItem>,
                                                   actualMedications: List<RecordItem>) {

        Assert.assertEquals(expectedMedications.count(), actualMedications.count())

        for (i in actualMedications.indices) {
            Assert.assertEquals(
                    LocalDate.parse(expectedMedications[i].date),
                    LocalDate.parse(actualMedications[i].label,
                            DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)
                    )
            )

            for (j in expectedMedications[i].lineItems.indices) {
                Assert.assertEquals(expectedMedications[i].lineItems[j].text, actualMedications[i].bodyElements[j])
            }
        }
    }
}