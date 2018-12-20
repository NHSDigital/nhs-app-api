package features.organDonation.stepDefinitions

import cucumber.api.java.en.When
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import pages.MorePage
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

    @When("^I navigate to the internal Organ Donation Choice Page")
    fun iNavigateToTheInternalOrganDonationChoicePage(){
        navBarNative.select(NavBarNative.NavBarType.MORE)
        browser.appendSourceQueryString("ios")
        morePage.btnOrganDonation.click()
        organDonationChoicePage.waitForSpinnerToDisappear()
        organDonationChoicePage.organDonationTitle.assertIsVisible()
    }

    @When("^I choose to not donate my organs")
    fun iChooseToNotDonateMyOrgans(){
        val button = organDonationChoicePage.noButton.assertIsVisible()
        button.click()
    }
}
