package features.ndop.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.stepDefinitions.AbstractDemographicsStepDefinitions
import net.serenitybdd.core.Serenity
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.ndop.NdopResponse

open class NdopStepDefinitions : AbstractDemographicsStepDefinitions() {

    val HTTP_EXCEPTION = "HttpException"

    @When("I request a Ndop Token")
    fun whenIRequestaNdopToken()
    {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getNdopToken(null)

            Serenity.setSessionVariable(NdopResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("I receive a signed JWT Token")
    fun thenIReceiveASignedJwtToken() {
        val result = Serenity.sessionVariableCalled<NdopResponse>(NdopResponse::class)
        Assert.assertTrue(result.response.token.isNotEmpty())
    }

}
