package pages.linkedProfiles.shutterPages

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.navigation.WebHeader
import pages.sharedElements.expectedPage.ExpectedPageStructure
import pages.sharedElements.expectedPage.ExpectedPageStructureAssertor
import pages.sharedElements.expectedPage.ParsedPage

@DefaultUrl("http://web.local.bitraft.io:3000/linked-profiles/shutter/appointments")
class AppointmentsShutterPage : HybridPageObject() {

    private lateinit var webHeader: WebHeader

    fun isLoaded(patientName: String) {
        webHeader.waitForPageHeaderText(
                "You do not have access to $patientName's GP appointments")
    }

    fun assertText(patientName: String) {
        val expected = ExpectedPageStructure()
                .paragraph("Contact $patientName's GP surgery for more information.")
                .paragraph("For urgent medical advice, go to 111.nhs.uk or call 111.")
                .h2("If you think $patientName might have coronavirus")
                .paragraph("They must stay at home and avoid close contact with other people.")
                .paragraph("Find out what to do if you think they might have coronavirus")
                .paragraph("Switch to your profile to book appointments for yourself.")
                .button("Switch to my profile")

        val parsedPage = ParsedPage.parse(this,"//div[p[@id=\"shutter-summary-text\"]]")
        ExpectedPageStructureAssertor().assert(parsedPage, expected.build())
    }
}
