package pages.linkedProfiles.shutterPages

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/linked-profiles/shutter/settings")
class SettingsShutterPage : ShutterComponent() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded() {
        headerNative.waitForPageHeaderText("Settings")
    }

    fun assertSwitchText() {
        Assert.assertEquals(
                "Switch text does not match",
                "Switch to your profile to access your settings.",
                switchText.textValue
        )
    }
}
