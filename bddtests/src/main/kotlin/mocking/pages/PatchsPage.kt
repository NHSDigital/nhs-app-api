package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class PatchsPage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1",
        page = this
    ).withText("PATCHS")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
