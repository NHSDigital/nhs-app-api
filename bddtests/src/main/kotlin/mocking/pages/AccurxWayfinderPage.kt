package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class AccurxWayfinderPage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1",
        page = this
    ).withText("AccurxWayfinder")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
