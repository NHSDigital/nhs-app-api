package features.myrecord.steps

import net.thucydides.core.annotations.Step
import pages.myrecord.MyRecordInfoPage
import pages.myrecord.MyRecordNoAccessPage
import pages.myrecord.MyRecordWarningPage

open class MyRecordSteps {

    lateinit var myRecordWarningPage: MyRecordWarningPage
    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Step
    fun isBackToHomePresent(): Boolean {
        return myRecordWarningPage.isBackToHomePresent()
    }

    @Step
    fun isAgreePresent(): Boolean {
        return myRecordWarningPage.isAgreePresent()
    }

    @Step
    fun getHeaderText(): String {
        return myRecordWarningPage.getHeaderText()
    }

    @Step
    fun clickAgreeandContinue() {
        myRecordWarningPage.clickAgreeandContinue()
    }

    @Step
    fun clickBacktoHome() {
        myRecordWarningPage.clickBacktoHome()
    }

    @Step
    fun warningText(): String {
        return myRecordWarningPage.warningText()
    }

    @Step
    fun isWarningMsgHighlighted(): String {
        return myRecordWarningPage.isWarningMsgHighlighted()
    }

    @Step
    fun getSensitiveList(): ArrayList<String> {
        return myRecordWarningPage.getSensitiveList()
    }

    @Step
    fun isOnMyRecordInfoPage(): Boolean {
        return myRecordInfoPage.isOnMyRecordInfoPage()
    }

    @Step
    fun clickMyDetails() {
        myRecordInfoPage.clickMyDetails()
    }

    @Step
    fun getNameLabelText(): String {
        return myRecordInfoPage.getNameLabelText()
    }

    @Step
    fun getDOBLabelText(): String {
        return myRecordInfoPage.getDOBLabelText()
    }

    @Step
    fun getSexLabelText(): String {
        return myRecordInfoPage.getSexLabelText()
    }

    @Step
    fun getAddressLabelText(): String {
        return myRecordInfoPage.getAddressLabelText()
    }

    @Step
    fun getNHSNumberLabelText(): String {
        return myRecordInfoPage.getNHSNumberLabelText()
    }

    @Step
    fun getMyDetailsLabelText(): String {
        return myRecordInfoPage.getMyDetailsLabelText()
    }

    @Step
    fun isNameVisible(): Boolean {
        return myRecordInfoPage.isNameVisible()
    }

    @Step
    fun getAllergiesAndAdverseReactionsHeaderText(): String {
        return myRecordInfoPage.getAllergiesAndAdverseReactionsHeaderText()
    }

    @Step
    fun getAllergyMessage(): String{
        return myRecordInfoPage.getAllergyMessage()
    }

    @Step
    fun isAllergiesTextMsgVisible(): Boolean {
        return myRecordInfoPage.isAllergiesTextMsgVisible()
    }

    @Step
    fun getAccessRevokedMessage(): String{
        return myRecordInfoPage.getAccessRevokedMessage()
    }

    @Step
    fun clickAllergiesAndAdverseReactionsSection(){
        return myRecordInfoPage.clickAllergiesAndAdverseReactionsSection()
    }

    @Step
    fun clickAcuteMedications() {
        myRecordInfoPage.clickAcuteMedications()
    }

    @Step
    fun getAcuteMedicationsHeaderText(): String {
        return myRecordInfoPage.getAcuteMedicationsHeaderText()
    }

    @Step
    fun getAcuteMedications(): String{
        return myRecordInfoPage.getAcuteMedications()
    }

    @Step
    fun clickImmunisations() {
        myRecordInfoPage.clickImmunisations()
    }

    @Step
    fun getImmunisationsHeaderText(): String {
        return myRecordInfoPage.getImmunistionsHeaderText()
    }

    @Step
    fun getImmunisationRecordCount(): Int {
        return myRecordInfoPage.getImmunisationRecordCount()
    }

    @Step
    fun getImmunisationsMessage(): String {
        return myRecordInfoPage.getImmunisationsMessage()
    }
}

