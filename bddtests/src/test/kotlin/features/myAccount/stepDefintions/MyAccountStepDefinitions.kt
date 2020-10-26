package features.myAccount.stepDefintions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.sharedSteps.BiometricSteps
import net.thucydides.core.annotations.Steps
import pages.account.MyAccountPage
import pages.assertElementNotPresent

class MyAccountStepDefinitions {

    lateinit var myAccount: MyAccountPage

    @Steps
    lateinit var biometricSteps: BiometricSteps

    @When("^I click the Notifications link on the Account page$")
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

    @Then("^the NHS login link is displayed$")
    fun theNHSLoginLinkIsDisplayed() {
        myAccount.assertNHSLoginLinkIsPresent()
    }

    @Then("^the Linked Profiles link is not displayed$")
    fun theLinkedProfilesLinkIsNotDisplayed() {
        myAccount.assertLinkedProfilesLinkIsNotPresent()
    }

    @Then("^the (.*) settings link is displayed$")
    fun theLoginAndPasswordOptionsLinkIsDisplayed(linkText: String) {
        when (linkText) {
            "Face ID" -> {
                biometricSteps.setBiometricType("face")
                myAccount.assertFaceIDIsPresent()
            }
            "Login options" -> myAccount.assertLoginAndPasswordOptionsIsPresent()
            "Touch ID" -> {
                biometricSteps.setBiometricType("touch")
                myAccount.assertTouchIDIsPresent()
            }
            "Fingerprint" -> {
                biometricSteps.setBiometricType("fingerPrint")
                myAccount.assertFingerprintIsPresent()
            }
        }
    }

    @Then("^I click the (.*) link on the settings page$")
    fun iClickTheLoginAndPasswordOptionsLink(linkText: String) {
        when (linkText) {
            "Face ID" -> myAccount.faceIDLink.click()
            "Login options" -> myAccount.loginAndPasswordOptionsLink.click()
            "Touch ID" -> myAccount.touchIDLink.click()
            "Fingerprint" -> myAccount.fingerprintLink.click()
            "NHS login" -> myAccount.nhsLoginLink.click()
            "Linked profiles" -> myAccount.linkedProfilesLink.click()
        }
    }

    @Then("^the Account Settings are available$")
    fun theAccountSettingsAreAvailable() {
        myAccount.settings.assertLinksPresent()
    }

    @Then("^there is no notification link available$")
    fun thereIsNoNotificationsLinkAvailable() {
        myAccount.notificationsLink.assertElementNotPresent()
    }
}
