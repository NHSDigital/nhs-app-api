package pages.loggedOut

import config.Config
import models.Patient
import pages.HybridPageObject
import pages.HybridPageElement

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

    fun isVisible() : Boolean {
        return createAccountButton.element.isVisible.and(mockPatientInput.element.isVisible)
    }

    fun completeAccountCreation(patient: Patient) {
        if(Config.instance.autoLogin != "true") {
            mockPatientInput.element.sendKeys(patient.hashCode().toString())
            hideKeyboardIfOnMobile()
            createAccountButton.click()
        }
    }
}
