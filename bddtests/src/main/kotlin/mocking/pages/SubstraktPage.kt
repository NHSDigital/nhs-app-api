package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class SubstraktPage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1",
            androidLocator = null,
            page = this
    ).withText("Substrakt")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
