package features.myrecord.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.ImmunisationsFactory
import mocking.data.myrecord.ImmunisationsData
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import pages.myrecord.MedicalRecordV1Page
import utils.SerenityHelpers
import utils.LinkedProfilesSerenityHelpers
import utils.getOrFail
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse
import java.time.LocalDate
import java.time.format.DateTimeFormatter

open class V1MedicalRecordImmunisationStepDefinitions : AbstractDemographicsStepDefinitions() {

    private lateinit var medicalRecordV1Page: MedicalRecordV1Page

    @Given("^the GP Practice has enabled immunisations functionality and multiple immunisation records exist$")
    fun givenTheGPPracticeHasEnabledImmunisationsFunctionalityAndMultipleRecordsExist() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ImmunisationsFactory.getForSupplier(gpSystem).enabledWithRecords(SerenityHelpers.getPatient())
    }

    @Given("^no immunisation records exist for the patient$")
    fun givenNoImmunisationRecordsExistForThePatient() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ImmunisationsFactory.getForSupplier(gpSystem).enabledWithBlankRecord(SerenityHelpers.getPatient())
    }

    @Given("^the user does not have access to view immunisations$")
    fun givenUserDoesNotHaveAccessToViewImmunisations() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ImmunisationsFactory.getForSupplier(gpSystem).noAccess(SerenityHelpers.getPatient())
    }

    @Given("^there is an error retrieving immunisations data$")
    fun givenThereIsAnErrorRetrievingImmunisationsData() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ImmunisationsFactory.getForSupplier(gpSystem).errorRetrieving(SerenityHelpers.getPatient())
    }

    @Given("^the EMIS GP Practice has two immunisation results where the first record has no date$")
    fun givenTheEmisGpPracticeHasAnImmunisationResultWithNoDate() {
        mockingClient.forEmis {
            myRecord.immunisationsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(ImmunisationsData.getTwoImmunisationResultsWhereTheFirstRecordHasNoDate())
        }
    }

    @When("^I get the users immunisations$")
    fun whenIGetTheUsersImmunisations() {
        try {
            val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail<String>()
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .myRecord.getMyRecord(patientId)

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I receive \"(.*)\" immunisations as part of the my record object$")
    fun thenIReceiveAnImmunisationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.immunisations.data.count())
    }

    @Then("^I see immunisation records displayed - Medical Record v1$")
    fun thenISeeImmunisationRecordsDisplayedV1() {
        assertEquals(2, medicalRecordV1Page.immunisations.allRecordItems().count())
    }

    @Then("^I see the expected immunisations displayed - Medical Record v1$")
    fun thenISeeTheExpectedImmunisationsDisplayedV1() {
        val expectedImmunisations = ImmunisationsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .getExpectedImmunisations()

        val onScreenImmunisations = medicalRecordV1Page.immunisations.allRecordItems()

        assertEquals(expectedImmunisations.count(), onScreenImmunisations.count())

        for (i in onScreenImmunisations.indices) {
            assertEquals(
                    LocalDate.parse(
                            expectedImmunisations[i].effectiveDate.value),
                    LocalDate.parse(
                            onScreenImmunisations[i].label,
                            DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)
                    ))

            assertEquals(expectedImmunisations[i].term, onScreenImmunisations[i].bodyElements[0])
            assertEquals(expectedImmunisations[i].nextDate, onScreenImmunisations[i].bodyElements[1])
            assertEquals(expectedImmunisations[i].status, onScreenImmunisations[i].bodyElements[2])
        }
    }
}
