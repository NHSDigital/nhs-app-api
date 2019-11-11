package pages.linkedProfiles.shutterPages

import net.thucydides.core.annotations.DefaultUrl
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/linked-profiles/shutter/more")
class MoreShutterPage : ShutterComponent() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded() {
        headerNative.waitForPageHeaderText("More")
    }
}
