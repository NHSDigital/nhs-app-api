package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class NetCompanyPage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1",
            androidLocator = null,
            page = this
    ).withText("Net Company")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
