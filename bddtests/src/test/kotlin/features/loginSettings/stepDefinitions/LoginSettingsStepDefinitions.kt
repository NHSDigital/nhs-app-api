package features.loginSettings.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import pages.account.LoginSettingsErrorPage
import pages.account.LoginSettingsPage

class LoginSettingsStepDefinitions {

    lateinit var loginSettings: LoginSettingsPage
    lateinit var loginSettingsError: LoginSettingsErrorPage

    @Steps
    lateinit var browser: BrowserSteps

    @Then("I see the (.*) settings page")
    fun iSeeTheLoginAndPasswordOptionsPage(biometricType: String) {
        loginSettings.assertTitleDisplayed(biometricType);
    }

    @When("I click the Login with (.*) toggle")
    fun iClickTheBiometricToggle(biometricType: String) {
        when (biometricType) {
            "Face ID" -> loginSettings.faceIDToggle.click()
            "Touch ID" -> loginSettings.touchIDToggle.click()
            "Fingerprint" -> loginSettings.fingerprintToggle.click()
        }
    }

    @Then("I see my (.*) registration was successful")
    fun iSeeMyRegistrationWasSuccessful(biometricType: String) {
        browser.setBiometricCompletionResult("Register", "Success","")
        loginSettings.assertToggleChecked(biometricType)
    }

    @Then("I see my (.*) deregistration was successful")
    fun iSeeMyDeregistrationWasSuccessful(biometricType: String) {
        browser.setBiometricCompletionResult("Deregister", "Success","")
        loginSettings.assertToggleNotChecked(biometricType)
    }

    @Given("I have already registered for biometrics")
    fun iAmAlreadyRegistered() {
        browser.setBiometricCompletionResult("Register", "Success","")
    }

    @Then("I see my (.*) deregistration was unsuccessful as it could not be found")
    fun iSeeMyDeregistrationWasUnsuccessfulAsItCouldNotBeFound(biometricType: String) {
        browser.setBiometricCompletionResult("Deregister", "Failed","10004")
        loginSettingsError.assertCannotFindContentDisplayed(biometricType)
    }

    @Then("I see my (.*) deregistration was unsuccessful as it could not be changed")
    fun iSeeMyDeregistrationWasUnsuccessfulAsItCouldNotBeChanged(biometricType: String) {
        browser.setBiometricCompletionResult("Deregister", "Failed","10005")
        loginSettingsError.assertCannotChangeContentDisplayed(biometricType)
    }
}
