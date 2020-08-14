package features.loginSettings.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.sharedSteps.BiometricSteps
import net.thucydides.core.annotations.Steps
import pages.account.LoginSettingsErrorPage
import pages.account.LoginSettingsPage

class LoginSettingsStepDefinitions {

    lateinit var loginSettings: LoginSettingsPage
    lateinit var loginSettingsError: LoginSettingsErrorPage

    @Steps
    lateinit var biometricSteps: BiometricSteps

    @Then("^I see the (.*) settings page$")
    fun iSeeTheLoginAndPasswordOptionsPage(biometricType: String) {
        loginSettings.assertTitleDisplayed(biometricType)
    }

    @When("^I click the Login with (.*) toggle$")
    fun iClickTheBiometricToggle(biometricType: String) {
        when (biometricType) {
            "Face ID" -> loginSettings.faceIDToggle.click()
            "Touch ID" -> loginSettings.touchIDToggle.click()
            "Fingerprint" -> loginSettings.fingerprintToggle.click()
        }
    }

    @Then("^I see my (.*) registration was successful$")
    fun iSeeMyRegistrationWasSuccessful(biometricType: String) {
        biometricSteps.setBiometricCompletionResult("Register", "Success","")
        loginSettings.assertToggleChecked(biometricType)
    }

    @Then("^I see my (.*) deregistration was successful$")
    fun iSeeMyDeregistrationWasSuccessful(biometricType: String) {
        biometricSteps.setBiometricCompletionResult("Deregister", "Success","")
        loginSettings.assertToggleNotChecked(biometricType)
    }

    @Given("^I have already registered for biometrics$")
    fun iAmAlreadyRegistered() {
        biometricSteps.setBiometricCompletionResult("Register", "Success","")
    }

    @Then("^I see my (.*) deregistration was unsuccessful as it could not be found$")
    fun iSeeMyDeregistrationWasUnsuccessfulAsItCouldNotBeFound(biometricType: String) {
        biometricSteps.setBiometricCompletionResult("Deregister", "Failed","10004")
        loginSettingsError.assertCannotFindContentDisplayed(biometricType)
    }

    @Then("^I see my (.*) deregistration was unsuccessful as it could not be changed$")
    fun iSeeMyDeregistrationWasUnsuccessfulAsItCouldNotBeChanged(biometricType: String) {
        biometricSteps.setBiometricCompletionResult("Deregister", "Failed","10005")
        loginSettingsError.assertCannotChangeContentDisplayed(biometricType)
    }
}
