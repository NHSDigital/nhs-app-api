package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class NetcallPage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1",
        page = this
    ).withText("Netcall")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
