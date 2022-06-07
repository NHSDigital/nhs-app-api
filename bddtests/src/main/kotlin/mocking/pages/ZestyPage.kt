package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class ZestyPage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1",
        page = this
    ).withText("Zesty")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
