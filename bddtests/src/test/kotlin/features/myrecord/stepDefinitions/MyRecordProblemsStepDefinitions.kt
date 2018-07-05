package features.myrecord.stepDefinitions

import cucumber.api.java.en.*
import features.myrecord.mockData.ProblemsData
import mocking.MockingClient
import mocking.defaults.MockDefaults
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.models.myrecord.MyRecordResponse
import worker.NhsoHttpException
import worker.WorkerClient

open class MyRecordProblemsStepDefinitions: AbstractDemographicsStepDefinitions() {

    @Given("the GP Practice has enabled problems functionality and the patient has 3 problems")
    fun givenTheGPPracticeHasEnabledProblemsFunctionalityAndPatientHasSomeProblems(count: Int) {
        mockingClient.forEmis {
            problemsRequest(this@MyRecordProblemsStepDefinitions.patient).respondWithSuccess(ProblemsData.getProblemsData())
        }
    }

    @Given("the GP Practice has enabled problems functionality for (.*)")
    fun givenTheGPPracticeHasEnabledProblemsFunctionalityFor(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    problemsRequest(this@MyRecordProblemsStepDefinitions.patient).respondWithSuccess(ProblemsData.getProblemsData())
                }
            }
            "TPP"->{
            }
        }
    }

    @Given("the GP Practice has enabled problems functionality and has 3 different problems with different date formats")
    fun givenTheGPPracticeHasEnabledProblemsFunctionalityAndHasThreeDifferentProblemsWithDifferentDateFormats() {
        mockingClient.forEmis {
            problemsRequest(this@MyRecordProblemsStepDefinitions.patient).respondWithSuccess(ProblemsData.getProblemRecordsWithDifferentDateParts())
        }
    }

    @But("the GP Practice has disabled problems functionality for (.*)")
    fun butTheGPPracticeHasDisabledProblemsFunctionality(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    problemsRequest(this@MyRecordProblemsStepDefinitions.patient).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP"->{
            }
        }
    }
    @Given("no Problems records exist for the patient for (.*)")
    fun givenNoProblemsRecordsExistForThePatient(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    problemsRequest(this@MyRecordProblemsStepDefinitions.patient).respondWithSuccess(ProblemsData.getDefaultProblemModel())
                }
            }
            "TPP"->{
            }
        }
    }

    @Given("the user does not have access to view Problems for (.*)")
    fun givenUserDoesNotHaveAccessToViewProblems(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    problemsRequest(this@MyRecordProblemsStepDefinitions.patient).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP"->{
            }
        }
    }

    @Given("there is an error retrieving Problems data for (.*)")
    fun givenThereIsAnErrorRetrievingProblemsData(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    problemsRequest(this@MyRecordProblemsStepDefinitions.patient).respondWithNonDataAccessException()
                }
            }
            "TPP"->{
            }
        }
    }

    @When("I get the users Problems")
    fun whenIGetTheUsersMyRecordData()
    {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getMyRecord(null)

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("I receive \"(.*)\" problems as part of the my record object")
    fun thenIReceiveAnProblemsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(count, result.response.problems.data.count())
    }

    @And("the flag informing that the patient has access to the problem data is set to \"(.*)\"")
    fun andHasAccessToProblemsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.problems.hasAccess)
    }

    @And("the flag informing that there was an error retrieving the problem data is set to \"(.*)\"")
    fun andHasErrorsWhenRetrievingProblemsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.problems.hasErrored)
    }
}
