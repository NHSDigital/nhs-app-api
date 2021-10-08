package pages.ndop

import pages.HybridPageObject
import pages. HybridPageElement
import pages.assertIsDisplayed

class NdopPage : HybridPageObject() {

    private val token = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),'token')]",
        page = this
    )

    fun assertTokenIsDisplayed() {
        token.assertIsDisplayed("Expected token to be displayed")
    }
}
