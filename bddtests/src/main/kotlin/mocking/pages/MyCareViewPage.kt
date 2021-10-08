package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class MyCareViewPage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1",
        page = this
    ).withText("My Care View")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
