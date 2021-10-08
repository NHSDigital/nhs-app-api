package pages.linkedProfiles.shutterPages

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.navigation.WebHeader
import pages.sharedElements.expectedPage.ExpectedPageStructure

@DefaultUrl("http://web.local.bitraft.io:3000/linked-profiles/shutter/advice")
class AdviceShutterPage :  HybridPageObject() {

    private lateinit var webHeader: WebHeader

    fun isLoaded() {
        webHeader.waitForPageHeaderText("Advice")
    }

    fun assertText(patientName: String) {
        ExpectedPageStructure()
                .paragraph("It's not possible to get health information " +
                        "and advice while acting on $patientName's behalf.")
                .paragraph("Switch to your profile to get advice.")
                .button("Switch to my profile")
                .assert(this)
    }
}
