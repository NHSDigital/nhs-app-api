package pages.throttling

import pages.HybridPageObject
import pages.HybridPageElement

open class NHSLoginPage : HybridPageObject() {

    val continueToCreateLoginButton = HybridPageElement(
            webDesktopLocator = "//button[@type='submit']",
            androidLocator = null,
            page = this
    )

    fun isVisible() : Boolean {
        return continueToCreateLoginButton.element.isVisible
    }
}
