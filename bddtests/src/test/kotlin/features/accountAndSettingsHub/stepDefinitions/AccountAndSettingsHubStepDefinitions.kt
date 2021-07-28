package features.accountAndSettingsHub.stepDefinitions

import io.cucumber.java.en.Then
import features.sharedSteps.BiometricSteps
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import pages.accountAndSettings.AccountAndSettingsPage
import pages.accountAndSettings.AccountAndSettingsLoginSettingsPage

class AccountAndSettingsHubStepDefinitions {

    lateinit var accountAndSettingsPage: AccountAndSettingsPage
    lateinit var accountAndSettingsLoginSettings: AccountAndSettingsLoginSettingsPage

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var biometricSteps: BiometricSteps

    @Then("^the Account and settings Hub page is displayed$")
    fun theAccountAndSettingsHubPageIsDisplayed() {
        accountAndSettingsPage.assertDisplayed()
    }

    @Then("^the Manage NHS login account link is displayed$")
    fun theManageNHSLoginAccountLinkIsDisplayed() {
        accountAndSettingsPage.assertManageNHSLoginAccountIsPresent()
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
        }
    }

    @Then("^I click the (.*) link on the account and settings page$")
    fun iClickTheLinkOnTheAccountAndSettingsPage(linkText: String) {
        when (linkText) {
            "Face ID" -> accountAndSettingsPage.faceIDLink.click()
            "Login options" -> accountAndSettingsPage.loginAndPasswordOptionsLink.click()
            "Touch ID" -> accountAndSettingsPage.touchIDLink.click()
            "Fingerprint" -> accountAndSettingsPage.fingerprintLink.click()
            "Manage NHS login account" -> accountAndSettingsPage.manageNhsLoginAccountLink.click()
            "Manage notifications" -> accountAndSettingsPage.manageNotificationsLink.click()
            "Legal and cookies" -> accountAndSettingsPage.legalAndCookiesLink.click()
        }
    }

    @Then("^I see the account and settings (.*) biometric page$")
    fun iSeeTheAccountAndSettingsBiometricPage(biometricType: String) {
        accountAndSettingsLoginSettings.assertTitleDisplayed(biometricType)
    }

    @Then("^The biometrics page url ends with (.*)$")
    fun theBiometricsPageUrlEndsWith(biometricUrl: String) {
        browser.shouldEndWithUrl(biometricUrl)
    }

    @Then("^the Account and settings page settings links are available$")
    fun theAccountAndSettingsPageSettingsLinksAreAvailable() {
        accountAndSettingsPage.settings.assertLinksPresent()
    }
}
