package pages.linkedProfiles.shutterPages

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.navigation.HeaderNative
import pages.text

@DefaultUrl("http://web.local.bitraft.io:3000/linked-profiles/shutter/symptoms")
class SymptomsShutterPage : ShutterComponent() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded() {
        headerNative.waitForPageHeaderText("Symptoms")
    }

    fun assertSummaryText(patientName: String) {
        Assert.assertEquals(
                "Failed to match summary text",
                "It's not possible to check your symptoms while acting on $patientName's behalf.",
                summaryText.text)
    }

    fun assertSwitchText() {
        Assert.assertEquals(
                "Switch text does not match",
                "Switch to your profile to check your symptoms.",
                switchText.textValue
        )
    }
}
