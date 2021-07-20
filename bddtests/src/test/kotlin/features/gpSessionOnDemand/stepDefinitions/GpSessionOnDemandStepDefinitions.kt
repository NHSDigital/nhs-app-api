package features.gpSessionOnDemand.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import models.Patient
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import worker.WorkerClient
import worker.models.session.UserSessionRequest

class GpSessionOnDemandStepDefinitions {

    @Then("^NHS Login returns an invalid Subject upon establishing a (.*) GP session$")
    fun nhsLoginReturnsAnInvalidSubjectUponEstablishingAGPSession(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
            "NHS_LOGIN_PATIENT_SUBJECT_OVERRIDE", "invalid-${Patient.getDefault(supplier).subject}")
    }

    @Given("^I have a valid GP Session$")
    fun iHaveAValidGpSession() {
        val patient = SerenityHelpers.getPatient()
        val ssoRedirectUri = GlobalSerenityHelpers.GP_SESSION_REDIRECT_URI.getOrFail<String>()
        Assert.assertNotNull(Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .postSessionConnection(UserSessionRequest(
                        authCode =patient.authCode,
                        codeVerifier = patient.codeVerifier,
                        redirectUrl = ssoRedirectUri)))
    }
}
