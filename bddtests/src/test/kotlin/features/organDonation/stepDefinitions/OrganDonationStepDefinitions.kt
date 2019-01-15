package features.organDonation.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import pages.organDonation.OrganDonationChoicePage
import java.net.URL

private const val NEW_TAB_WAIT_TIME = 1000L

open class OrganDonationStepDefinitions {

    @Steps
    lateinit var navbarSteps: NavigationSteps
    @Steps
    lateinit var browser: BrowserSteps

    lateinit var header: HeaderNative

    lateinit var organDonationChoicePage: OrganDonationChoicePage


    @Given("^I am a (.*) user not registered with organ donation, who wishes to register$")
    fun iAmNotRegisteredWithOrganDonationWishToRegister(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.unregisteredUser()
    }

    @Given("^the organ donation toggle is set to target the internal page$")
    fun toggleOrganDonationSiteLink() {
        browser.appendSourceQueryString("ios")
    }

    @Then("^the external Organ Donation page is displayed$")
    fun iAmOnTheExternalOrganDonationPage() {
        if (header.onMobile()) {
            pageOpensWithinNativeApp()
        } else {
            aNewTabOpens(Config.instance.organDonation)
        }
    }

    fun iAmOnTheOrganDonationPage() {
        iAmOnTheExternalOrganDonationPage()
    }

    @Then("^the internal Organ Donation page is displayed$")
    fun iAmOnTheInternalOrganDonationPage() {
        organDonationChoicePage.waitForSpinnerToDisappear()
        organDonationChoicePage.organDonationTitle.assertIsVisible()
    }

    private fun aNewTabOpens(url: String) {
        Thread.sleep(NEW_TAB_WAIT_TIME)
        browser.changeTab(URL(url))
        browser.shouldHaveUrl(url)
    }

    private fun pageOpensWithinNativeApp() {
        header.waitForNativeStepToComplete()
        header.waitForPageHeaderText("Organ donation register")
        navbarSteps.assertSelectedTab(NavBarNative.NavBarType.MORE)
    }
}
