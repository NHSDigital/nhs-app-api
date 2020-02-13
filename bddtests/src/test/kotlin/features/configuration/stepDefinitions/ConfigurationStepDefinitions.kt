package features.configuration.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.configuration.ConfigurationResponse

class ConfigurationStepDefinitions {

    @When("^I get the v1 configuration$")
    fun iGetTheV1Configuration() {
        try {
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .configuration
                    .getConfiguration("1.0.0","ios")

            ConfigurationSerenityHelpers.CONFIGURATION_RESPONSE.set(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @Then("^the configuration response will have a isDeviceSupported property$")
    fun theConfigurationResponseWillHaveAIsDeviceSupportedProperty() {
        val response = ConfigurationSerenityHelpers.CONFIGURATION_RESPONSE.getOrFail<ConfigurationResponse>()

        Assert.assertNotNull("Configuration response expected, but was null", response)
        Assert.assertNotNull("IsDeviceSupported property is not populated", response.isDeviceSupported)
    }

    @Then("^the configuration response will have a fidoServerUrl property$")
    fun theConfigurationResponseWillHaveAFidoServerUrlProperty() {
        val response = ConfigurationSerenityHelpers.CONFIGURATION_RESPONSE.getOrFail<ConfigurationResponse>()

        Assert.assertNotNull("Configuration response expected, but was null", response)
        Assert.assertNotNull("fidoServerUrl property is not populated", response.fidoServerUrl)
    }
}