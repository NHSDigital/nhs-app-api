package features.myrecord.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.But
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.ProblemsFactory
import mocking.data.myrecord.ProblemsData
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import pages.myrecord.MyRecordInfoPage
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

private const val NUMBER_OF_PROBLEMS_RECORDS_DISPLAYED = 3
open class MyRecordProblemsStepDefinitions: AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Given("^the GP Practice has enabled problems functionality for (.*)$")
    fun givenTheGPPracticeHasEnabledProblemsFunctionalityFor(getService: String) {
        setPatientToDefaultFor(getService)
        ProblemsFactory.getForSupplier(getService).enabledWithRecords(patient)
    }

    @Given("^the GP Practice has enabled problems functionality " +
            "and has 3 different problems with different date formats$")
    fun givenTheGPPracticeHasEnabledProblemsFunctionalityAndHasThreeDifferentProblemsWithDifferentDateFormats() {
        mockingClient.forEmis {
            myRecord.problemsRequest(this@MyRecordProblemsStepDefinitions.patient)
                    .respondWithSuccess(ProblemsData.getProblemRecordsWithDifferentDateParts())
        }
    }

    @But("^the GP Practice has disabled problems functionality for (.*)$")
    fun butTheGPPracticeHasDisabledProblemsFunctionality(getService: String) {
        setPatientToDefaultFor(getService)
        ProblemsFactory.getForSupplier(getService).disabled(patient)
    }
    @Given("^no Problems records exist for the patient for (.*)$")
    fun givenNoProblemsRecordsExistForThePatient(getService: String) {
        setPatientToDefaultFor(getService)
        ProblemsFactory.getForSupplier(getService).enabledWithBlankRecord(patient)
    }

    @Given("^the user does not have access to view Problems for (.*)$")
    fun givenUserDoesNotHaveAccessToViewProblems(getService: String) {
        setPatientToDefaultFor(getService)
        ProblemsFactory.getForSupplier(getService).noAccess(patient)
    }

    @Given("^there is an error retrieving Problems data for (.*)$")
    fun givenThereIsAnErrorRetrievingProblemsData(getService: String) {
        setPatientToDefaultFor(getService)
        ProblemsFactory.getForSupplier(getService).errorRetrieving(patient)
    }

    @When("^I get the users Problems$")
    fun whenIGetTheUsersMyRecordData()
    {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).myRecord.getMyRecord()

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I receive \"(.*)\" problems as part of the my record object$")
    fun thenIReceiveAnProblemsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.problems.data.count())
    }

    @And("^the flag informing that the patient has access to the problem data is set to \"(.*)\"$")
    fun andHasAccessToProblemsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.problems.hasAccess)
    }

    @And("^the flag informing that there was an error retrieving the problem data is set to \"(.*)\"$")
    fun andHasErrorsWhenRetrievingProblemsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.problems.hasErrored)
    }

    @Then("^I see Problems records displayed$")
    fun thenISeeProblemsRecordsDisplayed() {
        assertEquals(NUMBER_OF_PROBLEMS_RECORDS_DISPLAYED, myRecordInfoPage.problems.allRecordItems().count())
    }
}
