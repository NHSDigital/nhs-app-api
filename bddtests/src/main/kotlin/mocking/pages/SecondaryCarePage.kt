package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class SecondaryCarePage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1",
        page = this
    ).withText("Secondary Care")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
