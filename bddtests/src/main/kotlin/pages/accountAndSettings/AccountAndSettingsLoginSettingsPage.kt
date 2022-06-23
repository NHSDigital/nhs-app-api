package pages.accountAndSettings

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.sharedElements.ToggleElement

@DefaultUrl("http://web.local.bitraft.io:3000/more/account-and-settings/login-settings")
class AccountAndSettingsLoginSettingsPage : HybridPageObject() {

    val faceIDToggle = ToggleElement(this, "Log in with Face ID", "updateBiometricReg")
    val touchIDToggle = ToggleElement(this, "Log in with Touch ID", "updateBiometricReg")
    val fingerprintToggle = ToggleElement(this, "Log in with Fingerprint", "updateBiometricReg")

    private val titleLocator = "//h1[normalize-space(text())='%s']"

    fun assertTitleDisplayed(biometricType: String) {
        when (biometricType) {
            "undefined" -> assertDisplayWithNoInfoText(String.format(titleLocator, "Login options"))
            "Touch ID" -> assertDisplayWithNoInfoText(String.format(titleLocator, "Touch ID"))
            "Face ID" -> assertDisplayWithNoInfoText(String.format(titleLocator, "Face ID"))
            "Fingerprint" -> assertDisplayWithNoInfoText(String.format(titleLocator, "Fingerprint"))
        }
    }

    private fun assertDisplayWithNoInfoText(titleLocator: String) {
        val title = getTitle(titleLocator)
        title.assertIsVisible()
    }

    private fun getTitle(titleLocator: String): HybridPageElement {
        val title by lazy {
            HybridPageElement(
                titleLocator,
                page = this,
                helpfulName = "header")
        }
        return title
    }
}

