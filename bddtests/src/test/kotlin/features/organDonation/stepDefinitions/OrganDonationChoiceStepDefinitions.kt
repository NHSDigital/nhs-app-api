package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.BrowserSteps
import features.sharedSteps.PageUrl
import net.thucydides.core.annotations.Steps
import pages.MorePage
import pages.assertIsVisible
import pages.navigation.NavBarNative
import pages.organDonation.OrganDonationChoicePage

open class OrganDonationChoiceStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    lateinit var morePage: MorePage
    lateinit var organDonationChoicePage: OrganDonationChoicePage
    lateinit var navbarNative: NavBarNative

    @When("^I navigate to the internal Organ Donation Page")
    fun iNavigateToTheInternalOrganDonationPage() {
        if (!morePage.onMobile()) {
            val url = PageUrl.getPage("more")
            browser.browseTo(url)
        } else {
            navbarNative.select(NavBarNative.NavBarType.MORE)
        }
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

    @Then("^the amend Organ Donation Choice Page is displayed")
    fun theAmendOrganDonationChoicePageIsDisplayed() {
        organDonationChoicePage.assertDisplayed(amend = true)
    }
}
