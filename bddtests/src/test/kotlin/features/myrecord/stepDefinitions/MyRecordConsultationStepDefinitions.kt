package features.myrecord.stepDefinitions

import cucumber.api.java.en.*
import features.myrecord.mockData.ConsultationsData
import mocking.MockingClient
import mocking.defaults.MockDefaults
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

open class MyRecordConsultationStepDefinitions {

    @Steps
    val mockingClient = MockingClient.instance
    val HTTP_EXCEPTION = "HttpException"

    @Given("the GP Practice has multiple consultations for (.*)")
    fun givenTheGpPracticeHasMultipleConsultationsFor(getService:String) {
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    consultationsRequest(MockDefaults.patient).respondWithSuccess(ConsultationsData.getMultipleConsultationRecords())
                }
            }
            "TPP" -> {

            }
        }
    }

    @Given("the Patient has no access to consultations for (.*)")
    fun givenTheGPPracticeHasNoAccessToConsultations(getService: String) {
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    consultationsRequest(MockDefaults.patient).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP" -> {

            }
        }
    }

    @Given("the GP Practice has no consultations for (.*)")
    fun givenThePracticeHasNoConsultationsFor(getService: String) {
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    consultationsRequest(MockDefaults.patient).respondWithSuccess(ConsultationsData.getDefaultConsultationsData())
                }
            }
            "TPP" -> {

            }
        }
    }

    @Given("an error occurred retrieving the consultations for (.*)")
    fun givenAnErrorOccurredRetrievingTestResultsFrom(getService: String) {
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    consultationsRequest(MockDefaults.patient).respondWithNonDataAccessException()
                }
            }
            "TPP" -> {

            }
        }
    }

    @When("I get the users consultations")
    fun whenIGetTheUsersMyRecordData()
    {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getMyRecord(null)

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("I receive \"(.*)\" consultations as part of the my record object")
    fun thenIReceiveATestResultsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(count, result.response.consultations.data.count())
    }

    @Then("^I receive consultations object with hasAccess flag set to \"(.*)\"$")
    fun thenIReceiveConsultationsWithHasAccessFlagSetTo(hasAccess: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(hasAccess, result.response.consultations.hasAccess)
    }

    @Then("^the consultations object with hasErrored flag set to \"(.*)\"$")
    fun thenIReceiveConsultationsWithErrorFlagSetTo(hasError: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(hasError, result.response.consultations.hasErrored)
    }
}

