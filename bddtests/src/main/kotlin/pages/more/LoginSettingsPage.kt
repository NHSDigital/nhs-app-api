package pages.more

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.sharedElements.ToggleElement

@DefaultUrl("http://web.local.bitraft.io:3000/more/login-settings")
class LoginSettingsPage : HybridPageObject() {

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

    fun assertToggleChecked(biometricType: String) {
        when (biometricType) {
            "Face ID" -> faceIDToggle.assertOn()
            "Touch ID" -> touchIDToggle.assertOn()
            "Fingerprint" -> fingerprintToggle.assertOn()
        }
    }

    fun assertToggleNotChecked(biometricType: String) {
        when (biometricType) {
            "Face ID" -> faceIDToggle.assertOff()
            "Touch ID" -> touchIDToggle.assertOff()
            "Fingerprint" -> fingerprintToggle.assertOff()
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
                    titleLocator,
                    page = this,
                    helpfulName = "header")
        }
        return title
    }
}

