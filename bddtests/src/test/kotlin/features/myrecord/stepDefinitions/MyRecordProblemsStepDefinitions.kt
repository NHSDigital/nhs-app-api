package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.ProblemsFactory
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers
import worker.models.myrecord.MyRecordResponse

private const val NUMBER_OF_PROBLEMS_RECORDS_DISPLAYED = 3
open class MyRecordProblemsStepDefinitions: AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Given("^the GP Practice has enabled problems functionality$")
    fun givenTheGPPracticeHasEnabledProblemsFunctionalityFor() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        ProblemsFactory.getForSupplier(getService).enabledWithRecords(patient)
    }

    @Given("^the GP Practice has disabled problems functionality$")
    fun butTheGPPracticeHasDisabledProblemsFunctionality() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        ProblemsFactory.getForSupplier(getService).disabled(patient)
    }
    @Given("^no Problems records exist for the patient$")
    fun givenNoProblemsRecordsExistForThePatient() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        ProblemsFactory.getForSupplier(getService).enabledWithBlankRecord(patient)
    }

    @Given("^there is an error retrieving Problems data$")
    fun givenThereIsAnErrorRetrievingProblemsData() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        ProblemsFactory.getForSupplier(getService).errorRetrieving(patient)
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
    fun thenIReceiveAnProblemsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.problems.data.count())
    }

    @Then("^I see Problems records displayed$")
    fun thenISeeProblemsRecordsDisplayed() {
        assertEquals(NUMBER_OF_PROBLEMS_RECORDS_DISPLAYED, myRecordInfoPage.problems.allRecordItems().count())
    }
}
