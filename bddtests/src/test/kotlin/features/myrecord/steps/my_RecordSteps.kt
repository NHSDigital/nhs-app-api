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
    fun getAllergyMessage(): String {
        return myRecordInfoPage.getAllergyMessage()
    }

    @Step
    fun getNoAllergyMessage(): String {
        return myRecordInfoPage.getNoAllergyMessage()
    }

    @Step
    fun getNoAcuteMedicationMsg(): String {
        return myRecordInfoPage.getNoAcuteMedicationMsg()
    }

    @Step
    fun getNoCurrentRepeatMedicationMsg(): String {
        return myRecordInfoPage.getNoCurrentRepeatMedicationMsg()
    }

    @Step
    fun getNoDiscontinuedRepeatMedicationMsg(): String {
        return myRecordInfoPage.getNoDiscontinuedRepeatMedicationMsg()
    }

    @Step
    fun getAllergyCount(): Int {
        return myRecordInfoPage.getAllergyCount()
    }

    @Step
    fun getAllergyMessages(): ArrayList<String> {
        return myRecordInfoPage.getAllergyMessages()
    }

    @Step
    fun getAllergyDates(): ArrayList<String> {
        return myRecordInfoPage.getAllergyDates()
    }

    @Step
    fun isAllergiesTextMsgVisible(): Boolean {
        return myRecordInfoPage.isAllergiesTextMsgVisible()
    }

    @Step
    fun getTestResultsMessage(): String {
        return myRecordInfoPage.getTestResultsMessage()
    }

    @Step
    fun getSummaryCareNoAccessMessage(): String {
        return myRecordInfoPage.getSummaryCareNoAccessMessage()
    }

    @Step
    fun clickAllergiesAndAdverseReactionsSection() {
        return myRecordInfoPage.clickAllergiesAndAdverseReactionsSection()
    }

    @Step
    fun clickAcuteMedications() {
        myRecordInfoPage.clickAcuteMedications()
    }

    @Step
    fun clickCurrentRepeatMedications() {
        myRecordInfoPage.clickCurrentRepeatMedications()
    }

    @Step
    fun clickDiscontinuedRepeatMedications() {
        myRecordInfoPage.clickDiscontinuedRepeatMedications()
    }

    @Step
    fun getAcuteMedicationsHeaderText(): String {
        return myRecordInfoPage.getAcuteMedicationsHeaderText()
    }

    @Step
    fun getAcuteMedications(): String {
        return myRecordInfoPage.getAcuteMedications()
    }

    @Step
    fun isAcuteMedicationsAvailable(): Boolean {
        return myRecordInfoPage.isAcuteMedicationsAvailable()
    }

    @Step
    fun isRepeatMedicationsAvailable(): Boolean {
        return myRecordInfoPage.isRepeatMedicationsAvailable()
    }

    @Step
    fun isDiscontinuedMedicationsAvailable(): Boolean {
        return myRecordInfoPage.isDiscontinuedMedicationsAvailable()
    }

    @Step
    fun getTestResultsHeaderText(): String {
        return myRecordInfoPage.getTestResultsHeaderText()
    }

    @Step
    fun clickTestResultsSection() {
        return myRecordInfoPage.clickTestResultsSection()
    }

    @Step
    fun isTestResultsTextMsgVisible(): Boolean {
        return myRecordInfoPage.isTestResultsTextMsgVisible()
    }

    @Step
    fun getTestResultCount(): Int {
        return myRecordInfoPage.getTestResultCount()
    }

    @Step
    fun getTestResultChildCount(): Int {
        return myRecordInfoPage.getTestResultChildCount()
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

    @Step
    fun clickProblems() {
        myRecordInfoPage.clickProblems()
    }

    @Step
    fun getProblemsHeaderText(): String {
        return myRecordInfoPage.getProblemsHeaderText()
    }

    @Step
    fun getProblemsRecordCount(): Int {
        return myRecordInfoPage.getProblemsRecordCount()
    }

    @Step
    fun getProblemsMessage(): String {
        return myRecordInfoPage.getProblemsMessage()
    }

}

