package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import pages.MorePage
import pages.assertIsVisible
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import pages.organDonation.OrganDonationChoicePage

open class OrganDonationChoiceStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    lateinit var navBarNative: NavBarNative
    lateinit var header: HeaderNative
    lateinit var morePage: MorePage
    lateinit var organDonationChoicePage: OrganDonationChoicePage

    @When("^I navigate to the internal Organ Donation Page")
    fun iNavigateToTheInternalOrganDonationPage() {
        navBarNative.select(NavBarNative.NavBarType.MORE)
        morePage.waitForSpinnerToDisappear()
        browser.appendSourceQueryString("ios")
        morePage.btnOrganDonation.click()
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

    @When("^I select the Find Out More About Organ Donation link")
    fun iSelectTheFindOutMoreLink() {
        organDonationChoicePage.findOutMoreLink.assertIsVisible().click()
    }
}
