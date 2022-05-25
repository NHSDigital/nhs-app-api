package pages.gpAtHand.externalSites

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import java.net.URL

class OneOneOneOnlinePage: HybridPageObject() {

    val url: URL = URL("http://stubs.local.bitraft.io/external/111")

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1",
        page = this
    ).withText("/home")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
