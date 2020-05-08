package pages.account

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.assertElementNotPresent

@DefaultUrl("http://web.local.bitraft.io:3000/account")
class MyAccountPage : HybridPageObject() {

    private val listMenuPath = "//ul[@data-purpose='settings-menu']//li//a/span/div/h2"

    val signOutButton = HybridPageElement(
            webDesktopLocator = "//a[@id='account-logout']",
            iOSLocator = "//button[@id='signout-button']",
            androidLocator = "//button[@id='signout-button']",
            page = this
    )

    val signOutButtonMobile = HybridPageElement(
            webDesktopLocator = "//button[@id='signout-button']",
            iOSLocator = "//button[@id='signout-button']",
            androidLocator = "//button[@id='signout-button']",
            page = this
    )

    val aboutUs = AccountAboutUsModule(this)

    val settings = AccountSettingsModule(this)

    private fun link(linkText: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$listMenuPath${String.format(containsTextXpathSubstring, linkText)}",
                androidLocator = null,
                page = this,
                helpfulName = "$linkText Link")
    }

    val linkedProfilesLink = link("Linked profiles")
    val cookieLink = link("Cookies")
    val loginAndPasswordOptionsLink = link("Login options")
    val faceIDLink = link("Face ID")
    val touchIDLink = link("Touch ID")
    val fingerprintLink = link("Fingerprint")
    val notificationsLink = link("Notifications")

    fun assertDisplayed() {
        aboutUs.assertLinksPresent(true)
        signOutButton.assertIsVisible()
    }

    fun assertDisplayedForMobile() {
        signOutButtonMobile.assertIsVisible()
        aboutUs.assertLinksPresent(true)
    }

    fun assertLinkedProfilesLinkIsPresent() {
        linkedProfilesLink.assertIsVisible()
    }

    fun assertLinkedProfilesLinkIsNotPresent() {
        linkedProfilesLink.assertElementNotPresent()
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
}

