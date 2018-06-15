package features.myrecord.stepDefinitions

import cucumber.api.java.en.*
import features.myrecord.AllergiesData
import features.myrecord.DemographicsData
import mocking.MockingClient
import mocking.defaults.MockDefaults
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.demographics.Demographics
import worker.models.demographics.DemographicsResponse
import worker.models.myrecord.MyRecordResponse

open class DemographicsStepDefinitions {

    @Steps
    val mockingClient = MockingClient.instance

    val HTTP_EXCEPTION = "HttpException"

    @When("I get the users demographic data")
    fun whenIGetTheUsersDemographicsData() {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getDemographics(null)

            Serenity.setSessionVariable(Demographics::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Given("the GP Practice has enabled demographics functionality")
    fun givenTheGPPracticeHasEnabledDemographicsFunctionality() {
        mockingClient.forEmis {
            demographicsRequest(MockDefaults.patient).respondWithSuccess(DemographicsData.getDemographicData())
        }
    }

    @But("the GP Practice has disabled demographics functionality")
    fun butTheGPPracticeHasDisabledDemographicsFunctionality() {
        try {
            mockingClient.forEmis {
                demographicsRequest(MockDefaults.patient).respondWithExceptionWhenNotEnabled()
            }

            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getDemographics(null)

            Serenity.setSessionVariable(DemographicsResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("I receive the demographic object")
    fun thenIReceiveADemographicObject() {
        val result = Serenity.sessionVariableCalled<Demographics>(Demographics::class)
        Assert.assertNotNull(result)
    }
}

