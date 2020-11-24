package pages.linkedProfiles.shutterPages

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.navigation.HeaderNative
import pages.sharedElements.expectedPage.ExpectedPageStructure

@DefaultUrl("http://web.local.bitraft.io:3000/linked-profiles/shutter/messages")
class MessagesShutterPage :  HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded() {
        headerNative.waitForPageHeaderText("Messages")
    }

    fun assertText(patientName: String) {
        ExpectedPageStructure()
                .paragraph("It's not possible to access this section while acting on $patientName's behalf.")
                .paragraph("Switch to your profile to access this section.")
                .button("Switch to my profile")
                .assert(this)
    }
}
