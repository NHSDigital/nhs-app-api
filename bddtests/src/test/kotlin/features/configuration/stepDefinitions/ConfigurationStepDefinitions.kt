package features.configuration.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.getOrFail
import utils.set
import worker.WorkerClient
import worker.models.configuration.ConfigurationResponse

class ConfigurationStepDefinitions {

    @When("^I get the v1 configuration$")
    fun iGetTheV1Configuration() {
        val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .configuration
                .getConfiguration("1.0.0", "ios")
        ConfigurationSerenityHelpers.CONFIGURATION_RESPONSE.set(response)
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
