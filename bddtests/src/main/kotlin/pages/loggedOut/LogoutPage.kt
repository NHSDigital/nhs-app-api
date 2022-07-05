package pages.loggedOut

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsDisplayed

@DefaultUrl("http://web.local.bitraft.io:3000/logout")
class LogoutPage : HybridPageObject(), LoginFunction {

    val logoutPageText = HybridPageElement(
        webDesktopLocator = "//p[contains(text(), 'Log in to your NHS account again to access services online.')]",
        page = this
    )

    val loginOrCreateAccountButton = HybridPageElement(
        webDesktopLocator = "//button[contains(text(), 'Continue with NHS login')]",
        page = this
    )

    val forSecurityYouAreAutoLoggedOutText = HybridPageElement(
        webDesktopLocator = "//p[@id='forSecurityYouAreAutoLoggedOutText']",
        page = this
    )

    val ifYourWereEnteringInformationText = HybridPageElement(
        webDesktopLocator = "//p[@id='ifYouWereEnteringInfoText']",
        page = this
    )

    fun shouldDisplaySessionExpiredContent() {
        forSecurityYouAreAutoLoggedOutText.assertIsDisplayed("Expected to have security paragraph displayed")
        ifYourWereEnteringInformationText.assertIsDisplayed("Expected to have info paragraph displayed")
    }

    override fun signIn() {
        loginOrCreateAccountButton.click()
    }
}

