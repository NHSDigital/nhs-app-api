package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class EngagePage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1",
            androidLocator = null,
            page = this
        ).withText("Engage")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
