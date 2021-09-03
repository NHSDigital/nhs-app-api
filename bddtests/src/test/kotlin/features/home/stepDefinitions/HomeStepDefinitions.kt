package features.home.stepDefinitions

import io.cucumber.java.en.Then
import features.sharedSteps.BiometricSteps
import io.cucumber.java.en.When
import net.thucydides.core.annotations.Steps
import pages.HomePage

class HomeStepDefinitions {

    lateinit var homePage: HomePage

    @Steps
    lateinit var biometricSteps: BiometricSteps

    @When("^I click the (.*) biometrics button$")
    fun iClickTheBiometricsButton(buttonText: String) {
        homePage.clickOnButtonContainingText(buttonText)
    }

    @Then("^the (.*) button is displayed$")
    fun theBiometricButtonIsDisplayed(buttonText: String) {
        when (buttonText) {
            "Set up Face ID" -> {
                biometricSteps.setBiometricType("face")
            }
            "Set up Touch ID" -> {
                biometricSteps.setBiometricType("touch")
            }
            "Set up fingerprint" -> {
                biometricSteps.setBiometricType("fingerPrint")
            }
            "Set up fingerprint, face or iris" -> {
                biometricSteps.setBiometricType("fingerPrintFaceOrIris")
            }
        }
    }
}
