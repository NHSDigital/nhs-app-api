package features.nominatedPharmacy.stepDefinitions

import cucumber.api.java.en.Then
import pages.nominatedPharmacy.NominatedPharmacyInterruptPage

class NominatedPharmacyInterruptStepDefinitions {

    private lateinit var nominatedPharmacyInterruptPage: NominatedPharmacyInterruptPage

    @Then("^I see the update nominated pharmacy interrupt page loaded$")
    fun iSeeUpdateNominatedPharmacyInterruptPageIsLoaded() {
        nominatedPharmacyInterruptPage.isLoaded(
                "Any outstanding prescriptions may still arrive at your current nominated pharmacy")
    }

    @Then("^I see the set nominated pharmacy interrupt page loaded$")
    fun iSeeSetNominatedPharmacyInterruptPageIsLoaded() {
        nominatedPharmacyInterruptPage.isLoaded("The pharmacy you choose is where your prescriptions will be sent")
    }

    @Then("^I click on the interrupt continue button$")
    fun iClickOnTheInterruptContinueButton() {
        nominatedPharmacyInterruptPage.continueButton.click()
    }
}
