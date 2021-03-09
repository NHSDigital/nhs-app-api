package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class CareInformationExchangePage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1",
            androidLocator = null,
            page = this
    ).withText("Care Information Exchange")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
