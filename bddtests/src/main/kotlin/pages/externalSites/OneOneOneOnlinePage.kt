package pages.externalSites

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import java.net.URL

class OneOneOneOnlinePage: HybridPageObject() {

    val url: URL = URL("http://stubs.local.bitraft.io/external/111")

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1",
            androidLocator = null,
            page = this
    ).withText("111 online")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
