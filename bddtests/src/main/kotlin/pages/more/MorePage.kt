package pages.more

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertElementNotPresent
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/more")
class MorePage : HybridPageObject() {

    private val listMenuPath = "//ul[@data-purpose='more-menu']//li//a/div/h2"

    val signOutButton = HybridPageElement(
        webDesktopLocator = "//a[@id='account-logout']",
        page = this
    )

    val signOutButtonMobile = HybridPageElement(
        webDesktopLocator = "//button[@id='signout-button']",
        page = this
    )

    private fun link(linkText: String): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator = "$listMenuPath${String.format(containsTextXpathSubstring, linkText)}",
            page = this,
            helpfulName = "$linkText Link")
    }

    val linkedProfilesLink = link("Linked profiles")
    val accountAndSettingsLink = link("Account and settings")
    val gncrAdminLink = link("Great North Care Record preferences")
    val helpAndSupportLink = link("Help and support")
    val cookieLink = link("Cookies")
    val loginAndPasswordOptionsLink = link("Login options")
    val faceIDLink = link("Face ID")
    val touchIDLink = link("Touch ID")
    val fingerprintLink = link("Fingerprint")
    val notificationsLink = link("Notifications")
    val nhsLoginLink = link("NHS login")

    fun assertDisplayed() {
        signOutButton.assertIsVisible()
    }

    fun assertDisplayedForMobile() {
        signOutButtonMobile.assertIsVisible()
    }

    fun assertLinkedProfilesLinkIsPresent() {
        linkedProfilesLink.assertIsVisible()
    }

    fun assertLinkedProfilesLinkIsNotPresent() {
        linkedProfilesLink.assertElementNotPresent()
    }

    fun assertAccountAndSettingsLinkIsPresent() {
        accountAndSettingsLink.assertIsVisible()
    }

    fun assertGNCRAdminLinkIsPresent() {
        gncrAdminLink.assertIsVisible()
    }

    fun assertGNCRAdminLinkIsNotPresent() {
        gncrAdminLink.assertElementNotPresent()
    }

    fun assertHelpAndSupportLinkIsPresent() {
        helpAndSupportLink.assertIsVisible()
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

    fun assertLoginAndPasswordOptionsIsNotPresent() {
        loginAndPasswordOptionsLink.assertElementNotPresent()
    }

    fun assertCookiesLinkIsPresent() {
        cookieLink.assertIsVisible()
    }

    fun assertNHSLoginLinkIsPresent() {
        nhsLoginLink.assertIsVisible()
    }

    fun getHeaderElement(title: String): HybridPageElement {
        val locator = "//h2[contains(text(),\"$title\")]"
        return HybridPageElement(
            webDesktopLocator = locator,
            page = this,
            helpfulName = title
        )
    }
}

