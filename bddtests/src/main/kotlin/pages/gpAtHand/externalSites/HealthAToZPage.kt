package pages.gpAtHand.externalSites

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import java.net.URL

class HealthAToZPage: HybridPageObject() {

    val url: URL = URL("http://stubs.local.bitraft.io/external/healthAtoZ")

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1",
        page = this
    ).withText("Health A to Z")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
