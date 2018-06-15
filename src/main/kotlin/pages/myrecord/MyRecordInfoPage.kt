package pages.myrecord

import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.PageObject
import net.serenitybdd.core.pages.WebElementFacade

class MyRecordInfoPage : PageObject() {

    @FindBy(xpath = "//h5[contains(text(),'My details')]")
    lateinit var secMyDetails: WebElementFacade

    @FindBy(xpath = "//*[@id='app']/header/h1")
    lateinit var lblHeader: WebElementFacade

    @FindBy(xpath = "//label[contains(text(),'Name')]")
    lateinit var nameLabel: WebElementFacade

    @FindBy(xpath = "//label[contains(text(),'Name')]/following-sibling::p[1]")
    lateinit var txtName: WebElementFacade

    @FindBy(xpath = "//label[contains(text(),'Date of birth')]")
    lateinit var dobLabel: WebElementFacade

    @FindBy(xpath = "//label[contains(text(),'Sex')]")
    lateinit var sexLabel: WebElementFacade

    @FindBy(xpath = "//label[contains(text(),'Address')]")
    lateinit var addressLabel: WebElementFacade

    @FindBy(xpath = "//label[contains(text(),'NHS number')]")
    lateinit var nhsNumberLabel: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Allergies and adverse reactions')]")
    lateinit var allergiesAndAdverseReactionsHeader: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Allergies and adverse reactions')]/following-sibling::div[1]")
    lateinit var txtAllergyMsg: WebElementFacade

    @FindBy(xpath = "")
    lateinit var txtAccessRevokedMsg: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Acute medications')]")
    lateinit var acuteMedicationsHeading: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Acute medications')]/following-sibling::div[1]")
    lateinit var acuteMedications: WebElementFacade

    fun isNameVisible(): Boolean {
        waitABit(2000)
        return txtName.isCurrentlyVisible
    }

    fun isAllergiesTextMsgVisible(): Boolean {
        waitABit(2000)
        return txtAllergyMsg.isCurrentlyVisible
    }

    fun isOnMyRecordInfoPage(): Boolean {
        secMyDetails.waitUntilVisible<WebElementFacade>()
        return secMyDetails.isPresent
    }

    fun getMyDetailsLabelText(): String {
        return secMyDetails.text
    }

    fun clickMyDetails() {
        secMyDetails.click()
    }

    fun getNameLabelText(): String {
        return nameLabel.text
    }

    fun getDOBLabelText(): String {
        return dobLabel.text
    }

    fun getSexLabelText(): String {
        return sexLabel.text
    }

    fun getAddressLabelText(): String {
        return addressLabel.text
    }

    fun getNHSNumberLabelText(): String {
        return nhsNumberLabel.text
    }

    fun getAllergiesAndAdverseReactionsHeaderText(): String {
        return allergiesAndAdverseReactionsHeader.text
    }

    fun clickAllergiesAndAdverseReactionsSection() {
        allergiesAndAdverseReactionsHeader.click()
    }

    fun getAllergyMessage(): String {
        return txtAllergyMsg.text
    }

    fun getAccessRevokedMessage(): String {
            return txtAccessRevokedMsg.text
    }

    fun clickAcuteMedications() {
        acuteMedicationsHeading.click()
    }

    fun getAcuteMedicationsHeaderText(): String {
        return acuteMedicationsHeading.text
    }

    fun getAcuteMedications(): String {
        return acuteMedications.text
    }

}

