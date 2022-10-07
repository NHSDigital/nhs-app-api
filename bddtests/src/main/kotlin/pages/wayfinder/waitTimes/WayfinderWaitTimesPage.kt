package pages.wayfinder.waitTimes

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/hospital-referrals-appointments/waiting-lists")
open class WayfinderWaitTimesPage : HybridPageObject() {

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"Waiting lists\")]",
        page = this,
        helpfulName = "Waiting lists - h1"
    )

    fun assertWayfinderWaitTimesTitleIsDisplayed() {
        pageTitle.assertIsVisible()
    }
}
