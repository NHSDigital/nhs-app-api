package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class AccuRxPage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1",
        page = this
    ).withText("AccuRx")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
