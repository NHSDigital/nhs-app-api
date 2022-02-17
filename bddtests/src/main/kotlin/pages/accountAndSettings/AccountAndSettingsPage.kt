package pages.accountAndSettings

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/more/account-and-settings/")
class AccountAndSettingsPage : HybridPageObject() {

    private val listMenuPath = "//ul[@data-purpose='account-and-settings-menu']//li//a/div/h2"

    private fun link(linkText: String): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator = "$listMenuPath${String.format(containsTextXpathSubstring, linkText)}",
            page = this,
            helpfulName = "$linkText Link")
    }

    val settings = AccountSettingsModule(this)

    val manageNhsAccountLink = link("Manage NHS account")
    val legalAndCookiesLink = link("Legal and cookies")
    val loginAndPasswordOptionsLink = link("Login options")
    val faceIDLink = link("Face ID")
    val touchIDLink = link("Touch ID")
    val fingerprintLink = link("Fingerprint")
    val fingerprintFaceOrIrisLink = link("Fingerprint, face or iris")
    val manageNotificationsLink = link("Manage notifications")

    fun assertDisplayed() {
        title.waitForElement()
        manageNhsAccountLink.assertIsVisible()
        legalAndCookiesLink.assertIsVisible()
    }

    fun assertLoginAndPasswordOptionsIsPresent() {
        loginAndPasswordOptionsLink.assertIsVisible()
    }

    fun assertFaceIDIsPresent() {
        faceIDLink.assertIsVisible()
    }

    fun assertTouchIDIsPresent() {
        touchIDLink.assertIsVisible()
    }

    fun assertFingerprintIsPresent() {
        fingerprintLink.assertIsVisible()
    }

    fun assertFingerprintFaceOrIrisIsPresent() {
        fingerprintFaceOrIrisLink.assertIsVisible()
    }

    fun assertManageNHSAccountIsPresent() {
        manageNhsAccountLink.assertIsVisible()
    }

    fun assertManageNotificationsIsPresent() {
        manageNotificationsLink.assertIsVisible()
    }

    fun assertLegalAndCookiesIsPresent() {
        legalAndCookiesLink.assertIsVisible()
    }

    val clickLegalAndCookies by lazy {
        legalAndCookiesLink.click()
    }

    private val title by lazy {
        HybridPageElement(
            "//h1[normalize-space(text())='Account and settings']",
            this,
            helpfulName = "header")
    }
}
