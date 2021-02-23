package features.more.stepDefintions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.sharedSteps.BiometricSteps
import net.thucydides.core.annotations.Steps
import pages.more.MorePage
import pages.assertElementNotPresent

class MoreStepDefinitions {

    lateinit var more: MorePage

    @Steps
    lateinit var biometricSteps: BiometricSteps

    @When("^I click the Notifications link on the More page$")
    fun iClickTheNotificationsLinkOnTheMorePage() {
        more.settings.notifications.click()
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

    @Then("^the Cookies link is displayed$")
    fun theCookiesLinkIsDisplayed() {
        more.assertCookiesLinkIsPresent()
    }

    @Then("^the NHS login link is displayed$")
    fun theNHSLoginLinkIsDisplayed() {
        more.assertNHSLoginLinkIsPresent()
    }

    @Then("^the Linked Profiles link is not displayed$")
    fun theLinkedProfilesLinkIsNotDisplayed() {
        more.assertLinkedProfilesLinkIsNotPresent()
    }

    @Then("^the (.*) more link is displayed$")
    fun theLoginAndPasswordOptionsLinkIsDisplayed(linkText: String) {
        when (linkText) {
            "Face ID" -> {
                biometricSteps.setBiometricType("face")
                more.assertFaceIDIsPresent()
            }
            "Login options" -> more.assertLoginAndPasswordOptionsIsPresent()
            "Touch ID" -> {
                biometricSteps.setBiometricType("touch")
                more.assertTouchIDIsPresent()
            }
            "Fingerprint" -> {
                biometricSteps.setBiometricType("fingerPrint")
                more.assertFingerprintIsPresent()
            }
        }
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

    @Then("^the More Settings links are available$")
    fun theMoreSettingsLinksAreAvailable() {
        more.settings.assertLinksPresent()
    }

    @Then("^there is no notification link available$")
    fun thereIsNoNotificationsLinkAvailable() {
        more.notificationsLink.assertElementNotPresent()
    }
}
