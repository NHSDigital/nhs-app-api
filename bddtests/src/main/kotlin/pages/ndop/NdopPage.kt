package pages.ndop

import pages.HybridPageObject
import pages. HybridPageElement
import pages.isDisplayed
import pages.isPresent

class NdopPage : HybridPageObject() {

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//p[contains(text(),'token')]",
            androidLocator = null,
            iOSLocator = "//*[@id=\"ndop-token-form\"]",
            page = this
    )

    fun tokenIsDisplayed(): Boolean {
        if (onMobile()){
            return pageTitle.isPresent
        } else {
            return pageTitle.isDisplayed
        }
    }
}
