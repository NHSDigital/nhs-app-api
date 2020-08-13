package features.nominatedPharmacy.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import pages.nominatedPharmacy.NominatedPharmacyChooseTypePage
import pages.nominatedPharmacy.NominatedPharmacyDspInterruptPage

class NominatedPharmacyOnlineOnlyStepDefinitions {

    private lateinit var nominatedPharmacyChooseTypePage: NominatedPharmacyChooseTypePage

    private lateinit var nominatedPharmacyDspInterruptPage: NominatedPharmacyDspInterruptPage

    @When("^I click on the DSP Interrupt Prescription Home link$")
    fun iClickTheDspInterruptPrescriptionHomeLink() {
        nominatedPharmacyDspInterruptPage.prescriptionsHomeLink.click()
    }

    @Then("^I see the choose type page is loaded$")
    fun iSeeChooseTypePageIsLoaded() {
        nominatedPharmacyChooseTypePage.isLoaded()
    }

    @When("^I select online pharmacy$")
    fun iSelectOnlinePharmacy() {
        nominatedPharmacyChooseTypePage.onlinePharmacyRadioButton.click()
    }

    @Then("^I click on the choose type continue button$")
    fun iClickOnTheChooseTypeContinueButton() {
        nominatedPharmacyChooseTypePage.continueButton.click()
    }

    @Then("^I see the dsp interrupt page is loaded$")
    fun iSeeDspInterruptPageIsLoaded() {
        nominatedPharmacyDspInterruptPage.isLoaded()
    }
}
