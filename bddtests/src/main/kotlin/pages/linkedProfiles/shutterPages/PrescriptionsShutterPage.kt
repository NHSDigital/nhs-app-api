package pages.linkedProfiles.shutterPages

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.navigation.HeaderNative
import pages.sharedElements.expectedPage.ExpectedPageStructure

@DefaultUrl("http://web.local.bitraft.io:3000/linked-profiles/shutter/prescriptions")
class PrescriptionsShutterPage :  HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded(patientName: String) {
        headerNative.waitForPageHeaderText(
                "You do not have access to $patientName's repeat prescriptions")
    }

    fun assertText(patientName: String) {
        ExpectedPageStructure()
                .paragraph("Contact $patientName's GP surgery to request access.")
                .paragraph("Switch to your profile to order repeat prescriptions for yourself.")
                .button("Switch to my profile")
                .assert(this)
    }
}
