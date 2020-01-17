package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.ProblemsFactory
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import pages.myrecord.MedicalRecordV1Page
import utils.SerenityHelpers
import worker.models.myrecord.MyRecordResponse

private const val NUMBER_OF_PROBLEMS_RECORDS_DISPLAYED = 3
open class V1MedicalRecordProblemsStepDefinitions: AbstractDemographicsStepDefinitions() {

    private lateinit var medicalRecordV1Page: MedicalRecordV1Page

    @Given("^the GP Practice has enabled problems functionality$")
    fun givenTheGPPracticeHasEnabledProblemsFunctionality() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ProblemsFactory.getForSupplier(gpSystem).enabledWithRecords(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has disabled problems functionality$")
    fun butTheGPPracticeHasDisabledProblemsFunctionality() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ProblemsFactory.getForSupplier(gpSystem).disabled(SerenityHelpers.getPatient())
    }
    @Given("^no Problems records exist for the patient$")
    fun givenNoProblemsRecordsExistForThePatient() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ProblemsFactory.getForSupplier(gpSystem).enabledWithBlankRecord(SerenityHelpers.getPatient())
    }

    @When("there is bad problems data returned")
    fun badProblemsDataReturned() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ProblemsFactory.getForSupplier(gpSystem).badDataResponse(SerenityHelpers.getPatient())
    }

    @Given("^there is an error retrieving Problems data$")
    fun givenThereIsAnErrorRetrievingProblemsData() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ProblemsFactory.getForSupplier(gpSystem).errorRetrieving(SerenityHelpers.getPatient())
    }

    @When("^the flag informing that the patient has access to the problem data is set to \"(.*)\"$")
    fun andHasAccessToProblemsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.problems.hasAccess)
    }

    @When("^the flag informing that there was an error retrieving the problem data is set to \"(.*)\"$")
    fun andHasErrorsWhenRetrievingProblemsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.problems.hasErrored)
    }

    @Then("^I receive \"(.*)\" problems as part of the my record object$")
    fun thenIReceiveAProblemsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.problems.data.count())
    }

    @Then("^I see health condition records displayed - Medical Record v1$")
    fun thenISeeProblemsRecordsDisplayedV1() {
        assertEquals(
                NUMBER_OF_PROBLEMS_RECORDS_DISPLAYED,
                medicalRecordV1Page.healthConditions.allRecordItems().count())
    }
}
