package pages.linkedProfiles.shutterPages

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.navigation.HeaderNative
import pages.text

@DefaultUrl("http://web.local.bitraft.io:3000/linked-profiles/shutter/prescriptions")
class PrescriptionsShutterPage : ShutterComponent() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded(patientName: String) {
        headerNative.waitForPageHeaderText(
                "You do not have access to $patientName's repeat prescriptions")
    }

    fun assertSummaryText(patientName: String) {
        Assert.assertEquals(
                "Failed to match summary text",
                "Contact $patientName's GP surgery to request access.",
                summaryText.text)
    }

    fun assertSwitchText() {
        Assert.assertEquals(
                "Switch text does not match",
                "Switch to your profile to order repeat prescriptions for yourself.",
                switchText.textValue
        )
    }
}
