package pages.loggedOut

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject

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

    override fun signIn() {
        loginOrCreateAccountButton.click()
    }
}

