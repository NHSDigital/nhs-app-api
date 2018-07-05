package features.myrecord.stepDefinitions

import cucumber.api.java.en.*
import features.myrecord.mockData.DemographicsData
import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.tpp.models.Error
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.demographics.Demographics

open class DemographicsStepDefinitions {

    @Steps
    val mockingClient = MockingClient.instance

    val HTTP_EXCEPTION = "HttpException"

    @When("I get the users demographic data")
    fun whenIGetTheUsersDemographicsDataFor() {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getDemographics(null)

            Serenity.setSessionVariable(Demographics::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Given("the GP Practice has enabled demographics functionality for (.*)$")
    fun givenTheGPPracticeHasEnabledDemographicsFunctionalityFor(getService: String) {
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    demographicsRequest(MockDefaults.patient).respondWithSuccess(DemographicsData.getEmisDemographicData())
                }
            }
            "TPP"->{
                mockingClient.forTpp {
                    patientSelectedPost(MockDefaults.tppUserSession).respondWithSuccess(DemographicsData.getTppDemographicsData())
                }
            }
        }
    }

    @Given("the GP Practice has disabled demographics functionality for (.*)")
    fun butTheGPPracticeHasDisabledDemographicsFunctionalityFor(getService: String) {
        when(getService){
            "EMIS"->{
                try {
                    mockingClient.forEmis {
                        demographicsRequest(MockDefaults.patient).respondWithExceptionWhenNotEnabled()
                    }
                } catch (httpException: NhsoHttpException) {
                    Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
                }
            }
            "TPP"->{
                try {
                    mockingClient.forTpp {
                        patientSelectedPost(MockDefaults.tppUserSession).respondWithError(Error("6", "Error Occurred", "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                    }
                } catch (httpException: NhsoHttpException) {
                    Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
                }
            }
        }
    }

    @Then("I receive the demographic object")
    fun thenIReceiveADemographicObject() {
        val result = Serenity.sessionVariableCalled<Demographics>(Demographics::class)
        Assert.assertNotNull(result)
    }
}

