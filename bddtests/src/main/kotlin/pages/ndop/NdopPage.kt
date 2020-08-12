package pages.ndop

import pages.HybridPageObject
import pages. HybridPageElement
import pages.assertIsDisplayed
import pages.assertSingleElementPresent

class NdopPage : HybridPageObject() {

    private val token = HybridPageElement(
            webDesktopLocator = "//p[contains(text(),'token')]",
            androidLocator = null,
            iOSLocator = "//*[@id=\"ndop-token-form\"]",
            page = this
    )

    fun assertTokenIsDisplayed() {
        if (onMobile()){
            token.assertSingleElementPresent()
        } else {
            token.assertIsDisplayed("Expected token to be displayed")
        }
    }
}
