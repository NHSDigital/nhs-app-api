package features.prescriptions.stepDefinitions

import cucumber.api.java.en.Then
import features.prescriptions.steps.PrescriptionsSteps
import net.thucydides.core.annotations.Steps

open class PrescriptionsStepDefinitions {
    @Steps
    lateinit var prescriptions: PrescriptionsSteps

    @Then("^I see prescriptions page loaded$")
    fun iSeePrecriptionsPageLoaded() {
        prescriptions.isLoaded()
    }

    @Then("^I see a message indicating that I have no repeat prescriptions$")
    fun noRepeatPrescriptionsMessage() {
        prescriptions.assertNoRepeatPrescriptionsMessageShown()
    }
}