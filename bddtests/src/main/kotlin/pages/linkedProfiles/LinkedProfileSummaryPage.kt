package pages.linkedProfiles

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/linked-profiles/summary")
class LinkedProfileSummaryPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    val switchProfileButton = HybridPageElement(
            webDesktopLocator = "//*[@id='btn-switch-profile']",
            page = this
    )

    fun isLoaded(patientName: String) {
        headerNative.waitForPageHeaderText("Switch to " + patientName + "\'s profile to act on their behalf")
    }
}