package features.authentication.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.BiometricSteps
import net.thucydides.core.annotations.Steps
import pages.loggedOut.LoginBiometricErrorPage

open class LoginBiometricErrorStepDefinitions {

   lateinit var loginBiometricError: LoginBiometricErrorPage

   @Steps
   lateinit var biometricSteps: BiometricSteps

   @Then("I see the login biometric error page is displayed")
   fun iSeeTheLoginAndPasswordOptionsPage() {
      loginBiometricError.assertDisplayed()
   }

   @When("I attempt biometric login and fail")
   fun iAttemptBiometricLoginAndFail() {
      biometricSteps.triggerBiometricLoginError()
   }
}
