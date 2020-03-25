package pages.linkedProfiles.shutterPages

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.navigation.HeaderNative
import pages.sharedElements.expectedPage.ExpectedPageStructure

@DefaultUrl("http://web.local.bitraft.io:3000/linked-profiles/shutter/appointments")
class AppointmentsShutterPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded(patientName: String) {
        headerNative.waitForPageHeaderText(
                "You do not have access to $patientName's GP appointments")
    }

    fun assertText(patientName: String) {
        ExpectedPageStructure()
                .paragraph("Contact $patientName's GP surgery for more information. " +
                        "For urgent medical advice, go to 111.nhs.uk or call 111.")
                .h2("If you think $patientName might have coronavirus")
                .paragraph("They must stay at home and avoid close contact with other people.")
                .paragraph("Use the 111 coronavirus service to find out what to do")
                .paragraph("Switch to your profile to book appointments for yourself.")
                .button("Switch to my profile")
                .assert(this)
    }
}
