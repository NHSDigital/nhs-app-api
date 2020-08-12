package pages.loggedOut

import config.Config
import models.Patient
import pages.HybridPageObject
import pages.HybridPageElement
import pages.assertIsVisible
import pages.sendKeys

open class CIDAccountCreationPage : HybridPageObject() {

    val mockPatientInput = HybridPageElement(
            webDesktopLocator = "//input[@name='mock_patient']",
            androidLocator = null,
            page = this
    )

    val createAccountButton = HybridPageElement(
            webDesktopLocator = "//input[@type='submit']",
            androidLocator = null,
            page = this
    )

    fun assertIsVisible() {
        createAccountButton.assertIsVisible()
        mockPatientInput.assertIsVisible()
    }

    fun completeAccountCreation(patient: Patient) {
        if(Config.instance.autoLogin != "true") {
            mockPatientInput.sendKeys(patient.hashCode().toString())
            hideKeyboardIfOnMobile()
            createAccountButton.click()
        }
    }
}
