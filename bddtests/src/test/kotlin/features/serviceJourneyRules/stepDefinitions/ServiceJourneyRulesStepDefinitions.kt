package features.serviceJourneyRules.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedStepDefinitions.backend.CommonSteps
import mocking.defaults.TppMockDefaults.Companion.TPP_ODS_CODE_NO_SJR_CONFIGURATION
import models.Patient
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.serviceJourneyRules.ServiceJourneyRulesResponse

open class ServiceJourneyRulesStepDefinitions {

    @Given("^I am a user whose ODS Code has a specific journey configuration set up$")
    fun iAmAUserWhoseODSCodeHasASpecificJourneyConfigurationSetUp() {
        // EMIS is used as "EMIS TEST SURGERY" exists within gpinfo.csv with the default odsCode
        SerenityHelpers.setGpSupplier("EMIS")
    }

    @Given("^I am a user whose ODS Code does not have specific journey configuration set up$")
    fun iAmAUserWhoseODSCodeDoesNotHaveASpecificJourneyConfigurationSetUp() {
        // TPP is used as the default odsCode does not currently exist within gpinfo.csv
        SerenityHelpers.setGpSupplier("TPP")
        val patient = Patient.getDefault("TPP")
        patient.odsCode = TPP_ODS_CODE_NO_SJR_CONFIGURATION
        patient.tppUserSession!!.unitId = TPP_ODS_CODE_NO_SJR_CONFIGURATION
        SerenityHelpers.setPatient(patient)
    }

    @When("^I request the service journey rules for my ODS Code$")
    fun iRequestTheServiceJourneyRulesForODSCode() {
        try {
            val workerClient = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
            val result = workerClient.serviceJourneyRules.getServiceJourneyRulesConfiguration()

            Serenity.setSessionVariable(ServiceJourneyRulesResponse::class).to(result)
            Serenity.setSessionVariable("Http Status Code").to(result)

        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @Then("^I receive the service journey rules response$")
    fun iReceiveServiceJourneyRulesOnTheResponse() {
        val serviceJourneyRulesResponse = Serenity
                .sessionVariableCalled<ServiceJourneyRulesResponse>(ServiceJourneyRulesResponse::class)

        Assert.assertNotNull("Expected serviceJourneyRulesResponse not null", serviceJourneyRulesResponse)
    }

    @When("^I login but service journey rules has no configuration for my GP practice")
    fun whenILoginButServiceJourneyRulesConfigurationHasNoConfiguration() {
        try {
            CommonSteps().givenIHaveLoggedInAndHaveAValidSessionCookie()
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }
}
