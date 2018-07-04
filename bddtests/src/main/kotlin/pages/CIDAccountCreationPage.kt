package pages

import models.Patient
import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.annotations.findby.How
import net.serenitybdd.core.pages.WebElementFacade

open class CIDAccountCreationPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {
    @FindBy(how = How.XPATH, using = "//input[@name='mock_patient']")
    lateinit var mockPatientInput: WebElementFacade

    @FindBy(how = How.XPATH, using = "//input[@type='submit']")
    lateinit var createAccountButton: WebElementFacade

    fun isVisible() : Boolean {
        return createAccountButton.isVisible
    }

    fun completeAccountCreation(patient: Patient) {
        mockPatientInput.sendKeys(patient.hashCode().toString())
        createAccountButton.click()
    }
}