package pages.account

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.HybridPageElement
import pages.assertIsVisible
@DefaultUrl("http://web.local.bitraft.io:3000/account")
class MyAccountPage : HybridPageObject() {

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

    val personalDetails = AccountPersonalDetailsModule(this)

    val aboutUs = AccountAboutUsModule(this)

    val settings = AccountSettingsModule(this)

    fun assertDisplayed() {
        aboutUs.assertLinksPresent(true)
        signOutButton.assertIsVisible()
    }

    fun assertDisplayedForMobile() {
        signOutButtonMobile.assertIsVisible()
        aboutUs.assertLinksPresent(true)
    }
}

