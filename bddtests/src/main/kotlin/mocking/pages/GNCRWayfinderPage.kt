package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class GNCRWayfinderPage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1",
        page = this
    ).withText("GNCRWayfinder")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
