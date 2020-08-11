package pages.externalSites

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import java.net.URL

class AdviceAboutCoronavirusPage: HybridPageObject() {

    val url: URL = URL("http://stubs.local.bitraft.io/external/covid")

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1",
            androidLocator = null,
            page = this
    ).withText("Advice About Coronavirus")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
