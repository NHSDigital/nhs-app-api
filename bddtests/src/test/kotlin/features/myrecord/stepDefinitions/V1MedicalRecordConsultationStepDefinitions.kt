package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.ConsultationsFactory
import mocking.MockingClient
import mocking.data.myrecord.ConsultationsData
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import pages.myrecord.MedicalRecordV1Page
import utils.SerenityHelpers
import utils.LinkedProfilesSerenityHelpers
import utils.getOrFail
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

open class V1MedicalRecordConsultationStepDefinitions {

    private lateinit var medicalRecordV1Page: MedicalRecordV1Page
    private val mockingClient = MockingClient.instance

    @Given("^the GP Practice has multiple consultations$")
    fun givenTheGpPracticeHasMultipleConsultations() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatient()

        ConsultationsFactory.getForSupplier(gpSystem).enabledWithRecords(patient)
    }

    @Given("^the Patient has no access to consultations$")
    fun givenThePatientHasNoAccessToConsultations() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatient()

        ConsultationsFactory.getForSupplier(gpSystem).disabled(patient)
    }

    @Given("^the GP Practice has no consultations$")
    fun givenThePracticeHasNoConsultations() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatient()

        ConsultationsFactory.getForSupplier(gpSystem).enabledWithBlankRecord(patient)
    }

    @Given("^the EMIS GP Practice has two consultations where the first record has no date$")
    fun givenTheEmisPracticeHasAConsultationWithNoDate(){
        val patient = SerenityHelpers.getPatient()
        mockingClient.forEmis {
            myRecord.consultationsRequest(patient)
                    .respondWithSuccess(ConsultationsData.getTwoConsultationsWhereTheFirstRecordHasNoDate())
        }
    }

    @Given("^an error occurred retrieving the consultations$")
    fun givenAnErrorOccurredRetrievingConsultations() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatient()

        ConsultationsFactory.getForSupplier(gpSystem).errorRetrieving(patient)
    }

    @Given("^the GP practice returns bad consultations data$")
    fun theGPPracticeReturnsACorruptedResponse() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatient()

        ConsultationsFactory.getForSupplier(gpSystem).recordWithBadConsultationsData(patient)
    }

    @When("I get the users consultations")
    fun whenIGetTheUsersConsultations() {
        try {
            val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail<String>()

            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .myRecord.getMyRecord(patientId)

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @Then("I receive \"(.*)\" consultations as part of the my record object")
    fun thenIReceiveATestResultsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.consultations.data.count())
    }

    @Then("^I receive consultations object with hasAccess flag set to \"(.*)\"$")
    fun thenIReceiveConsultationsWithHasAccessFlagSetTo(hasAccess: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(hasAccess, result.response.consultations.hasAccess)
    }

    @Then("^the consultations object with hasErrored flag set to \"(.*)\"$")
    fun thenIReceiveConsultationsWithErrorFlagSetTo(hasError: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(hasError, result.response.consultations.hasErrored)
    }

    @Then("^I see Consultations records displayed - Medical Record v1$")
    fun thenISeeConsultationsRecordsDisplayedV1() {
        assertEquals(2, medicalRecordV1Page.consultations.allRecordItems().count())
    }

    @Then("^I see (.*) Consultations records displayed - Medical Record v1$")
    fun thenISeeConsultationsRecordsDisplayedV1(count: Int) {
        assertEquals(count, medicalRecordV1Page.consultations.allRecordItems().count())
    }

}

