package pages.linkedProfiles.shutterPages

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.navigation.WebHeader
import pages.sharedElements.expectedPage.ExpectedPageStructure

@DefaultUrl("http://web.local.bitraft.io:3000/linked-profiles/shutter/more")
class MoreShutterPage :  HybridPageObject() {

    private lateinit var webHeader: WebHeader

    fun isLoaded() {
        webHeader.waitForPageHeaderText("More")
    }

    fun assertText(patientName: String) {
        ExpectedPageStructure()
                .paragraph("It's not possible to access this section while acting on $patientName's behalf.")
                .paragraph("Switch to your profile to access this section.")
                .button("Switch to my profile")
                .assert(this)
    }
}
