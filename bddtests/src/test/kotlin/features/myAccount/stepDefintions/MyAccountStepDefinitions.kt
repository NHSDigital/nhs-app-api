package features.myAccount.stepDefintions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.BrowserSteps
import features.sharedSteps.PageUrl
import net.thucydides.core.annotations.Steps
import pages.account.MyAccountPage
import utils.SerenityHelpers

class MyAccountStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps

    lateinit var myAccount: MyAccountPage

    @When("I navigate to the Account page for mobile devices")
    fun iNavigateToTheAccountPageForMobileDevices() {
        val url = PageUrl().getPageWithMobileSource("account")
        browser.browseTo(url)
        myAccount.assertDisplayedForMobile()
    }

    @When("I navigate to the Account page for desktop")
    fun iNavigateToTheAccountPageForDesktop() {
        val url = PageUrl().getPageWithoutSource("account")
        browser.browseTo(url)
        myAccount.assertDisplayed()
        myAccount.settings.assertNotDisplayed()
    }

    @When("I click the Notifications link on the Account page")
    fun iClickTheNotificationsLinkOnTheAccountPage(){
        myAccount.settings.notifications.click()
    }

    @Then("^the Account page for mobile devices is displayed$")
    fun theAccountPageForMobileDevicesIsDisplayed() {
        myAccount.assertDisplayedForMobile()
    }

    @Then("^the Account page is displayed$")
    fun theAccountPageIsDisplayed() {
        myAccount.assertDisplayed()
    }

    @Then("^I see my personal details")
    fun iSeeMyPersonalDetails() {
        val patient = SerenityHelpers.getPatient()
        myAccount.personalDetails.assertVisible(patient.formattedFullName(),
                patient.formattedDateOfBirth(),
                patient.formattedNHSNumber())
    }

    @Then("the Account Settings are available")
    fun theAccountSettingsAreAvailable(){
        myAccount.settings.assertLinksPresent()
    }

    @Then("there are no Account Settings available")
    fun thereAreNoAccountSettingsAvailable(){
        myAccount.settings.assertNotDisplayed()
    }
}
