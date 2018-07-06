package pages

import models.Patient

open class CIDAccountCreationPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {
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
        return createAccountButton.element.isVisible
    }

    fun completeAccountCreation(patient: Patient) {
        mockPatientInput.element.sendKeys(patient.hashCode().toString())
        if (onMobile()) {
            hideKeyboard()
        }
        createAccountButton.element.click()
    }
}