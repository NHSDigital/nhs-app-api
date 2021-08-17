package mocking.pages.help

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import java.net.URL

abstract class NhsAppHelpPage protected constructor(title: String): HybridPageObject() {

    val url = URL("http://stubs.local.bitraft.io/help/${title}")

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1",
            androidLocator = null,
            page = this
    ).withText(title, false)


    fun assertTitle() {
        pageTitle.assertIsVisible()
    }
}
