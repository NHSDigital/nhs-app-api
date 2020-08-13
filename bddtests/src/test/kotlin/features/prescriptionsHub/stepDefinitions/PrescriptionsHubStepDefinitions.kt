package features.prescriptionsHub.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import pages.PrescriptionsHubPage

class PrescriptionsHubStepDefinitions {

    private lateinit var prescriptionsHubPage: PrescriptionsHubPage

    @When("^I click the View Orders link$")
    fun clickViewOrdersLink() {
        prescriptionsHubPage.viewOrdersPanel.click()
    }

    @When("^I click the nominated pharmacy link on the Prescriptions Hub$")
    fun iClickTheNominatedPharmacyLinkOnThePrescriptionsHub() {
        prescriptionsHubPage.nominatedPharmacyLink.click()
    }

    @Then("^the Prescriptions Hub page is displayed$")
    fun assertIsDisplayed() {
        prescriptionsHubPage.assertPrescriptionsHubIsDisplayed()
    }

    @Then("^I click the Order a repeat prescription button$")
    fun clickOrderARepeatPrescriptionButton() {
        prescriptionsHubPage.clickOrderARepeatPrescriptionButton()
    }
}
