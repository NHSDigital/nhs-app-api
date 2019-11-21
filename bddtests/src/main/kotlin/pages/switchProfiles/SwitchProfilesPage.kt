package pages.switchProfiles

import models.switchProfiles.SwitchProfileData
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.HybridPageElement
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/switch-profile")
open class SwitchProfilesPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    val switchToMyProfileButton = HybridPageElement(
            webDesktopLocator = "//button[@id='switch-profile-button']",
            page = this
    ).withText("Switch to my profile", false)

    fun isLoaded(name: String) {
        headerNative.waitForPageHeaderText("You are acting on behalf of $name")
    }

    fun getDisplayedProxyDetails(): SwitchProfileData {

        val age = findByXpath("//span[@id='proxy-age']")
        val gpPracticeName = findByXpath("//span[@id='proxy-gp-practice']")

        return SwitchProfileData(age = age.text,
                gpPracticeName = gpPracticeName.text)
    }
}
