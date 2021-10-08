package features.organDonation.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import pages.assertIsVisible
import pages.gpMedicalRecord.MedicalRecordHubPage
import pages.navigation.WebHeader
import pages.organDonation.OrganDonationChoicePage

open class OrganDonationChoiceStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    lateinit var yourHealthPage: MedicalRecordHubPage
    lateinit var organDonationChoicePage: OrganDonationChoicePage
    lateinit var webHeader: WebHeader

    @When("^I navigate to the internal Organ Donation Page")
    fun iNavigateToTheInternalOrganDonationPage() {
        yourHealthPage.getHeaderElement("Manage your organ donation decision").click()
    }

    @When("^I navigate to the internal Organ Donation Choice Page")
    fun iNavigateToTheInternalOrganDonationChoicePage() {
        iNavigateToTheInternalOrganDonationPage()
        organDonationChoicePage.assertDisplayed()
    }

    @When("^I choose to not donate my organs")
    fun iChooseToNotDonateMyOrgans() {
        val button = organDonationChoicePage.noButton.assertIsVisible()
        button.click()
    }

    @When("^I choose to donate my organs")
    fun iChooseToDonateMyOrgans() {
        val button = organDonationChoicePage.yesButton.assertIsVisible()
        button.click()
    }

    @Then("^the internal Organ Donation Choice Page is displayed")
    fun theInternalOrganDonationChoicePageIsDisplayed() {
        organDonationChoicePage.assertDisplayed()
    }

    @Then("^the amend Organ Donation Choice Page is displayed")
    fun theAmendOrganDonationChoicePageIsDisplayed() {
        organDonationChoicePage.assertDisplayed(amend = true)
    }
}
