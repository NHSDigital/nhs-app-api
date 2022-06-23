package pages.more

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.sharedElements.ToggleElement

@DefaultUrl("http://web.local.bitraft.io:3000/more/login-settings")
class LoginSettingsPage : HybridPageObject() {

    val faceIDToggle = ToggleElement(this, "Log in with Face ID", "updateBiometricReg")
    val touchIDToggle = ToggleElement(this, "Log in with Touch ID", "updateBiometricReg")
    val fingerprintToggle = ToggleElement(this, "Log in with fingerprint", "updateBiometricReg")
    val fingerprintFaceOrIrisToggle = ToggleElement(this, "Log in with fingerprint, face or iris", "updateBiometricReg")

    fun assertToggleChecked(biometricType: String) {
        when (biometricType) {
            "Face ID" -> faceIDToggle.assertOn()
            "Touch ID" -> touchIDToggle.assertOn()
            "Fingerprint" -> fingerprintToggle.assertOn()
            "Fingerprint, face or iris" -> fingerprintFaceOrIrisToggle.assertOn()
        }
    }

    fun assertToggleNotChecked(biometricType: String) {
        when (biometricType) {
            "Face ID" -> faceIDToggle.assertOff()
            "Touch ID" -> touchIDToggle.assertOff()
            "Fingerprint" -> fingerprintToggle.assertOff()
            "Fingerprint, face or iris" -> fingerprintFaceOrIrisToggle.assertOff()
        }
    }
}

