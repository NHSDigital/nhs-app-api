package features.prescriptionsHub.stepDefinitions

import cucumber.api.java.en.When
import cucumber.api.java.en.Then
import pages.PrescriptionsHubPage
import pages.assertIsNotVisible

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

    @Then("^the 'Nominate a pharmacy' link is not available on the Prescriptions Hub$")
    fun theHospitalAndOtherServicesLinkIsNotAvailableOnTheAppointmentsHub() {
        prescriptionsHubPage.nominatedPharmacyLink.assertIsNotVisible()
    }

    @Then("^I click the Order a repeat prescription button$")
    fun clickOrderARepeatPrescriptionButton() {
        prescriptionsHubPage.clickOrderARepeatPrescriptionButton()
    }
}
