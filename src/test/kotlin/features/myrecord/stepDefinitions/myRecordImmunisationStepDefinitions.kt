package features.myrecord.stepDefinitions

import cucumber.api.java.en.*
import mocking.defaults.MockDefaults.Companion.patient
import mocking.MockingClient
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.myrecord.*
import features.myrecord.steps.MyRecordSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.defaults.MockDefaults
import mocking.emis.models.MedicationsResponse
import worker.models.demographics.DemographicsResponse
import worker.models.myrecord.MyRecordResponse
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient

open class MyRecordImmunisationStepDefinitions {

    @Steps
    val mockingClient = MockingClient.instance
    val HTTP_EXCEPTION = "HttpException"

    @Given("the GP Practice has enabled immunisations functionality")
    fun givenTheGPPracticeHasEnabledImmunisationsFunctionality() {

        mockingClient.forEmis {
            testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getMultipleTestResultsData())
        }

        mockingClient.forEmis {
            immunisationsRequest(patient).respondWithSuccess(ImmunisationsData.getImmunisationsData())
        }

        mockingClient.forEmis {
            allergiesRequest(patient).respondWithSuccess(AllergiesData.getAllergiesData(2))
        }

        mockingClient.forEmis {
            medicationsRequest(patient).respondWithSuccess(MedicationsData.getMedicationData())
        }
    }

    @When("I get the users immunisations")
    fun whenIGetTheUsersMyRecordData()
    {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getMyRecord(null)

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("I receive \"(.*)\" immunisations as part of the my record object")
    fun thenIReceiveAnImmunisationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(count, result.response.immunisations?.data?.count())
    }
}