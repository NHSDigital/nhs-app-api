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
import worker.models.configuration.ConfigurationV2Response

class ConfigurationV2StepDefinitions {

    @When("^I get the v2 configuration$")
    fun iGetTheV2Configuration() {
        try {
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .configuration
                    .getConfigurationv2()

            ConfigurationSerenityHelpers.CONFIGURATION_RESPONSE.set(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @Then("^the v2 configuration response will have a nhsLoginLoggedInPaths property$")
    fun theConfigurationResponseWillHaveANhsLoginLoggedInPathsProperty() {
        val response = ConfigurationSerenityHelpers.CONFIGURATION_RESPONSE.getOrFail<ConfigurationV2Response>()

        Assert.assertNotNull("Configuration response expected, but was null", response)
        Assert.assertNotNull("NhsLoginLoggedInPaths property is not populated",
                response.nhsLoginLoggedInPaths)
    }

    @Then("^the v2 configuration response will have a minimumSupportedAndroidVersion property$")
    fun theConfigurationResponseWillHaveAMinimumSupportedAndroidVersionProperty() {
        val response = ConfigurationSerenityHelpers.CONFIGURATION_RESPONSE.getOrFail<ConfigurationV2Response>()

        Assert.assertNotNull("Configuration response expected, but was null", response)
        Assert.assertNotNull("MinimumSupportedAndroidVersion property is not populated",
                response.minimumSupportedAndroidVersion)
    }

    @Then("^the v2 configuration response will have a minimumSupportediOSVersion property$")
    fun theConfigurationResponseWillHaveAMinimumSupportediOSVersionProperty() {
        val response = ConfigurationSerenityHelpers.CONFIGURATION_RESPONSE.getOrFail<ConfigurationV2Response>()

        Assert.assertNotNull("Configuration response expected, but was null", response)
        Assert.assertNotNull("MinimumSupportediOSVersion property is not populated",
                response.minimumSupportediOSVersion)
    }

    @Then("^the v2 configuration response will have a fidoServerUrl property$")
    fun theConfigurationResponseWillHaveAFidoServerUrlProperty() {
        val response = ConfigurationSerenityHelpers.CONFIGURATION_RESPONSE.getOrFail<ConfigurationV2Response>()

        Assert.assertNotNull("Configuration response expected, but was null", response)
        Assert.assertNotNull("fidoServerUrl property is not populated", response.fidoServerUrl)
    }

    @Then("^the v2 configuration response will have a knownServices property$")
    fun theConfigurationResponseWillHaveAKnownServicesProperty() {
        val response = ConfigurationSerenityHelpers.CONFIGURATION_RESPONSE.getOrFail<ConfigurationV2Response>()

        Assert.assertNotNull("Configuration response expected, but was null", response)
        Assert.assertNotNull("KnownServices property is not populated", response.knownServices)
        Assert.assertNotEquals("KnownServices property has values", response.knownServices.count(), 0 )
    }

}