package features.myAccount.stepDefintions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import pages.account.MyAccountPage
import pages.assertElementNotPresent

class MyAccountStepDefinitions {

    lateinit var myAccount: MyAccountPage

    @When("I click the Notifications link on the Account page")
    fun iClickTheNotificationsLinkOnTheAccountPage() {
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

    @Then("^the Linked Profiles link is displayed$")
    fun theLinkedProfilesLinkIsDisplayed() {
        myAccount.assertLinkedProfilesLinkIsPresent()
    }

    @Then("^the Cookies link is displayed$")
    fun theCookiesLinkIsDisplayed() {
        myAccount.assertCookiesLinkIsPresent()
    }

    @Then("^the Linked Profiles link is not displayed$")
    fun theLinkedProfilesLinkIsNotDisplayed() {
        myAccount.assertLinkedProfilesLinkIsNotPresent()
    }

    @Then("^the Login options link is displayed$")
    fun theLoginAndPasswordOptionsLinkIsDisplayed() {
        myAccount.assertLoginAndPasswordOptionsIsPresent()
    }

    @Then("^the Login options link is not displayed$")
    fun theLoginAndPasswordOptionsLinkIsNotDisplayed() {
        myAccount.assertLoginAndPasswordOptionsIsNotPresent()
    }

    @Then("^I click the Login options link$")
    fun iClickTheLoginAndPasswordOptionsLink() {
        myAccount.loginAndPasswordOptionsLink.click()
    }

    @Then("the Account Settings are available")
    fun theAccountSettingsAreAvailable() {
        myAccount.settings.assertLinksPresent()
    }

    @Then("there are no Account Settings available")
    fun thereAreNoAccountSettingsAvailable() {
        myAccount.settings.assertNotDisplayed()
    }

    @Then("there is no notification link available")
    fun thereIsNoNotificationsLinkAvailable() {
        myAccount.notificationsLink.assertElementNotPresent()
    }

    @Then("the link to Notifications is not available on the Account page")
    fun theLinkToNotificationsIsNotAvailableOnTheAccountPage() {
        myAccount.settings.assertNotDisplayed()
    }
}
