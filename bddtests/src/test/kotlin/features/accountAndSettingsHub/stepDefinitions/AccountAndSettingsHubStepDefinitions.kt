package features.accountAndSettingsHub.stepDefinitions

import io.cucumber.java.en.Then
import features.sharedSteps.BiometricSteps
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.accountAndSettings.AccountAndSettingsLoginSettingsPage
import pages.accountAndSettings.AccountAndSettingsPage
import pages.accountAndSettings.ExampleNotificationsPage
import pages.accountAndSettings.NotificationsSettingsPage
import pages.accountAndSettings.MoreThanOneDevicePage

class AccountAndSettingsHubStepDefinitions {

    lateinit var accountAndSettingsPage: AccountAndSettingsPage
    lateinit var accountAndSettingsLoginSettings: AccountAndSettingsLoginSettingsPage
    lateinit var manageNotificationsPage: NotificationsSettingsPage
    lateinit var exampleNotificationsPage: ExampleNotificationsPage
    lateinit var manageNotificationsForMoreThanOneDevice: MoreThanOneDevicePage
    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var biometricSteps: BiometricSteps

    @Then("^the Account and settings Hub page is displayed$")
    fun theAccountAndSettingsHubPageIsDisplayed() {
        accountAndSettingsPage.assertDisplayed()
    }

    @Then("^the Manage NHS account link is displayed$")
    fun theManageNHSAccountLinkIsDisplayed() {
        accountAndSettingsPage.assertManageNHSAccountIsPresent()
    }

    @Then("^the Manage notifications link is displayed$")
    fun theManageNotificationsLinkIsDisplayed() {
        accountAndSettingsPage.assertManageNotificationsIsPresent()
    }

    @Then("^the Legal and cookies link is displayed$")
    fun theLegalAndCookiesLinkIsDisplayed() {
        accountAndSettingsPage.assertLegalAndCookiesIsPresent()
    }

    @Then("^the (.*) account and settings link is displayed$")
    fun theBiometricTypeAccountAndSettingsLinkIsDisplayed(linkText: String) {
        when (linkText) {
            "Face ID" -> {
                biometricSteps.setBiometricType("face")
                accountAndSettingsPage.assertFaceIDIsPresent()
            }
            "Login options" -> accountAndSettingsPage.assertLoginAndPasswordOptionsIsPresent()
            "Touch ID" -> {
                biometricSteps.setBiometricType("touch")
                accountAndSettingsPage.assertTouchIDIsPresent()
            }
            "Fingerprint" -> {
                biometricSteps.setBiometricType("fingerPrint")
                accountAndSettingsPage.assertFingerprintIsPresent()
            }
            "Fingerprint, face or iris" -> {
                biometricSteps.setBiometricType("fingerPrintFaceOrIris")
                accountAndSettingsPage.assertFingerprintFaceOrIrisIsPresent()
            }
            else -> Assert.fail("Biometric type not specified")
        }
    }

    @Then("^I click the (.*) link on the account and settings page$")
    fun iClickTheLinkOnTheAccountAndSettingsPage(linkText: String) {
        when (linkText) {
            "Face ID" -> accountAndSettingsPage.faceIDLink.click()
            "Login options" -> accountAndSettingsPage.loginAndPasswordOptionsLink.click()
            "Touch ID" -> accountAndSettingsPage.touchIDLink.click()
            "Fingerprint" -> accountAndSettingsPage.fingerprintLink.click()
            "Fingerprint, face or iris" -> accountAndSettingsPage.fingerprintFaceOrIrisLink.click()
            "Manage NHS account" -> accountAndSettingsPage.manageNhsAccountLink.click()
            "Manage notifications" -> accountAndSettingsPage.manageNotificationsLink.click()
            "Legal and cookies" -> accountAndSettingsPage.legalAndCookiesLink.click()
        }
    }

    @Then("^I click the (.*) link on the manage notifications page$")
    fun iClickTheLinkOnManageNotificationsPage(linkText: String) {
        when (linkText) {
            "Example notifications" ->  manageNotificationsPage.exampleNotifications.click()
            "More than one device" ->  manageNotificationsPage.moreThanOneDevice.click()
        }
    }

    @Then("^the Example notifications page is displayed$")
    fun theExampleNotificationPageIsDisplayed() {
        exampleNotificationsPage.assertDisplayed()
    }

    @Then("^the Manage notifications for more than one device page is displayed$")
    fun theManageNotificationsForMoreThanOneDeviceIsDisplayed() {
        manageNotificationsForMoreThanOneDevice.assertDisplayed()
    }

    @Then("^I see the account and settings (.*) biometric page$")
    fun iSeeTheAccountAndSettingsBiometricPage(biometricType: String) {
        accountAndSettingsLoginSettings.assertTitleDisplayed(biometricType)
    }

    @Then("^the Account and settings page settings links are available$")
    fun theAccountAndSettingsPageSettingsLinksAreAvailable() {
        accountAndSettingsPage.settings.assertLinksPresent()
    }
}
