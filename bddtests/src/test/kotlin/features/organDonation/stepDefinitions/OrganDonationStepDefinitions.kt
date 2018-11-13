package features.organDonation.stepDefinitions

import config.Config
import cucumber.api.java.en.Then
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import java.net.URL

private const val NEW_TAB_WAIT_TIME = 1000L

open class OrganDonationStepDefinitions {

    @Steps
    lateinit var navbarSteps: NavigationSteps
    @Steps
    lateinit var browser: BrowserSteps
    lateinit var header: HeaderNative


    @Then("^I am on the Organ Donation page$")
    fun iAmOnTheOrganDonationPage() {
        if(header.onMobile()){
           pageOpensWithinNativeApp()
        } else {
            aNewTabOpens(Config.instance.organDonation)
        }

    }

    private fun aNewTabOpens(url: String) {
        //This wait has been added because local runs are failing when
        //chrome is not run in headless mode - race condition
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
