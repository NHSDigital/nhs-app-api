package pages.linkedProfiles.shutterPages

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.navigation.HeaderNative
import pages.sharedElements.expectedPage.ExpectedPageStructure

@DefaultUrl("http://web.local.bitraft.io:3000/linked-profiles/shutter/settings")
class SettingsShutterPage :  HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded() {
        headerNative.waitForPageHeaderText("Settings")
    }

    fun assertText() {
        ExpectedPageStructure()
                .paragraph("Switch to your profile to access your settings.")
                .button("Switch to my profile")
                .assert(this)
    }
}
