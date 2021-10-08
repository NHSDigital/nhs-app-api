package pages.linkedProfiles

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.navigation.WebHeader

@DefaultUrl("http://web.local.bitraft.io:3000/linked-profiles/summary")
class LinkedProfileSummaryPage : HybridPageObject() {

    private lateinit var webHeader: WebHeader

    val switchProfileButton = HybridPageElement(
            webDesktopLocator = "//*[@id='btn-switch-profile']",
            page = this
    )

    fun isLoaded(patientName: String) {
        webHeader.waitForPageHeaderText("Switch to " + patientName + "\'s profile to act on their behalf")
    }
}
