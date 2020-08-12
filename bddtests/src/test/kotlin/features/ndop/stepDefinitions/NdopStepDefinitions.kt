package features.ndop.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.LinkedProfilesSerenityHelpers
import utils.getOrFail
import worker.WorkerClient
import worker.models.ndop.NdopResponse

open class NdopStepDefinitions {

    @When("^I request a Ndop Token$")
    fun whenIRequestaNdopToken() {
        val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail<String>()
        val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .getNdopToken(patientId)
        Serenity.setSessionVariable(NdopResponse::class).to(result)
    }

    @Then("^I receive a signed JWT Token$")
    fun thenIReceiveASignedJwtToken() {
        val result = Serenity.sessionVariableCalled<NdopResponse>(NdopResponse::class)
        Assert.assertTrue(result.token.isNotEmpty())
    }
}
