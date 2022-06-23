package features.more.stepDefintions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import pages.more.MorePage
import pages.assertElementNotPresent

class MoreStepDefinitions {

    lateinit var more: MorePage

    @When("^I click the Account and settings link on the More page$")
    fun iClickTheAccountAndSettingsLinkOnTheMorePage() {
        more.accountAndSettingsLink.click()
    }

    @When("I click the account menu item '(.*)'$")
    fun clickMenuItem(title: String) {
        more.getHeaderElement(title).click()
    }

    @Then("^the More page for mobile devices is displayed$")
    fun theMorePageForMobileDevicesIsDisplayed() {
        more.assertDisplayedForMobile()
    }

    @Then("^the More page is displayed$")
    fun theMorePageIsDisplayed() {
        more.assertDisplayed()
    }

    @Then("^the Linked Profiles link is displayed$")
    fun theLinkedProfilesLinkIsDisplayed() {
        more.assertLinkedProfilesLinkIsPresent()
    }

    @Then("^the Linked Profiles link is not displayed$")
    fun theLinkedProfilesLinkIsNotDisplayed() {
        more.assertLinkedProfilesLinkIsNotPresent()
    }

    @Then("^the Account and Settings link is displayed$")
    fun theAccountAndSettingsLinkIsDisplayed() {
        more.assertAccountAndSettingsLinkIsPresent()
    }

    @Then("^the Help and Support link is displayed$")
    fun theHelpAndSupportLinkIsDisplayed() {
        more.assertHelpAndSupportLinkIsPresent()
    }

    @Then("^I click the (.*) link on the more page$")
    fun iClickTheLoginAndPasswordOptionsLink(linkText: String) {
        when (linkText) {
            "Face ID" -> more.faceIDLink.click()
            "Login options" -> more.loginAndPasswordOptionsLink.click()
            "Touch ID" -> more.touchIDLink.click()
            "Fingerprint" -> more.fingerprintLink.click()
            "NHS login" -> more.nhsLoginLink.click()
            "Linked profiles" -> more.linkedProfilesLink.click()
        }
    }

    @Then("^there is no notification link available$")
    fun thereIsNoNotificationsLinkAvailable() {
        more.notificationsLink.assertElementNotPresent()
    }
}
