package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class GNCRPage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1",
            androidLocator = null,
            page = this
        ).withText("GNCR")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
