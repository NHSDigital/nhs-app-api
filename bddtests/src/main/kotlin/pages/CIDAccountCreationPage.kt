package pages

import config.Config
import models.Patient

open class CIDAccountCreationPage : HybridPageObject() {
    val mockPatientInput = HybridPageElement(
            browserLocator = "//input[@name='mock_patient']",
            androidLocator = null,
            page = this
    )

    val createAccountButton = HybridPageElement(
            browserLocator = "//input[@type='submit']",
            androidLocator = null,
            page = this
    )

    fun isVisible() : Boolean {
        return createAccountButton.element.isVisible.and(mockPatientInput.element.isVisible)
    }

    fun completeAccountCreation(patient: Patient) {
        if(Config.instance.autoLogin != "true") {
            mockPatientInput.element.sendKeys(patient.hashCode().toString())
            if (onMobile()) {
                hideKeyboard()
            }
            createAccountButton.click()
        }
    }
}