package pages.myrecord

import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject

const val SHRUB_ANIMATION_DURATION_MILLIS: Long = 500

class MyRecordInfoPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val secMyDetails = 
        HybridPageElement(
            browserLocator = "//h2[contains(text(),'My details')]",
            androidLocator = null,
            page = this)

    val clinicalAbbreviationsLink =
            HybridPageElement(
                    browserLocator = "//a[contains(text(),'Help with abbreviations')]",
                    androidLocator = null,
                    page = this)

    val nameLabel = 
        HybridPageElement(
            browserLocator = "//label[contains(text(),'Name')]",
            androidLocator = null,
            page = this)

    val txtName = 
        HybridPageElement(
            browserLocator = "//label[contains(text(),'Name')]/following-sibling::p[1]",
            androidLocator = null,
            page = this)

    val dobLabel = 
        HybridPageElement(
            browserLocator = "//label[contains(text(),'Date of birth')]",
            androidLocator = null,
            page = this)

    val sexLabel = 
        HybridPageElement(
            browserLocator = "//label[contains(text(),'Sex')]",
            androidLocator = null,
            page = this)

    val addressLabel = 
        HybridPageElement(
            browserLocator = "//label[contains(text(),'Address')]",
            androidLocator = null,
            page = this)

    val nhsNumberLabel = 
        HybridPageElement(
            browserLocator = "//label[contains(text(),'NHS number')]",
            androidLocator = null,
            page = this)


    val allergies by lazy {MyRecordWrapper("Allergies and adverse reactions", this)}

    val noSummaryCareAccessMessage = 
        HybridPageElement(
            browserLocator = "//div[@id='mainDiv']//div[@data-purpose='error'][1]",
            androidLocator = null,
            page = this)


    val acuteMedications by lazy {MyRecordWrapper("Acute (short-term) medications", this)}

    val repeatMedications by lazy {MyRecordWrapper("Repeat medications: current", this)}

    val discontinuedRepeatMedications by lazy {MyRecordWrapper("Repeat medications: discontinued", this)}

    val testResults by lazy {MyRecordWrapper("Test results", this)}

    val immunisations by lazy {MyRecordWrapper("Immunisations", this)}

    val problems by lazy {MyRecordWrapper("Problems", this)}

    val consultations by lazy {MyRecordWrapper("Consultations", this)}

    val eventsHeading = 
        HybridPageElement(
            browserLocator = "//h2[contains(text(),'C')]",
            androidLocator = null,
            page = this)

    val events = 
        HybridPageElement(
            browserLocator = "//h2[contains(text(),'Consultations')]/following-sibling::div[1]",
            androidLocator = null,
            page = this)

    fun isNameVisible(): Boolean {
        return txtName.element.isCurrentlyVisible
    }

    fun isAllergiesTextMsgVisible(): Boolean {
        return allergies.firstParagraph.isCurrentlyVisible
    }

    fun isOnMyRecordInfoPage(): Boolean {
        return secMyDetails.element.isPresent
    }

    fun canSeeClinicalAbbreviationsLink(): Boolean {
        return clinicalAbbreviationsLink.element.isPresent
    }

    fun clickClinicalAbbreviationsLink() {
        toggleShrub(clinicalAbbreviationsLink)
    }

    fun getMyDetailsLabelText(): String {
        return secMyDetails.element.text
    }

    fun clickMyDetails() {
        toggleShrub(secMyDetails)
    }

    fun getNameLabelText(): String {
        return nameLabel.element.text
    }

    fun getDOBLabelText(): String {
        return dobLabel.element.text
    }

    fun getSexLabelText(): String {
        return sexLabel.element.text
    }

    fun getAddressLabelText(): String {
        return addressLabel.element.text
    }

    fun getNHSNumberLabelText(): String {
        return nhsNumberLabel.element.text
    }

    fun getAllergiesAndAdverseReactionsHeaderText(): String {
        return allergies.header.element.text
    }

    fun clickAllergiesAndAdverseReactionsSection() {
        allergies.toggleShrub()
    }

    fun getAllergyMessage(): String {
        return allergies.msg.element.text
    }

    fun getNoAllergyMessage(): String {
        return allergies.firstParagraph.text
}

    fun getNoAcuteMedicationMsg(): String {
        return acuteMedications.firstParagraph.text
    }

    fun getNoCurrentRepeatMedicationMsg(): String {
        return repeatMedications.firstParagraph.text
    }

    fun getNoDiscontinuedRepeatMedicationMsg(): String {
        return discontinuedRepeatMedications.firstParagraph.text
    }

    fun getAllergyCount(): Int {
        return allergies.allRecordItems().count()
    }

    fun getAllergyMessages(): ArrayList<String> {
        val msgs = ArrayList<String>()
        val list = allergies.allRecordItemBodies()
        for (item in list) {
            msgs.add(item.text)
        }
        return msgs
    }

    fun getAllergyDates(): ArrayList<String> {
        val msgs = ArrayList<String>()
        val list = allergies.allRecordItemLabels()
        for (item in list) {
            msgs.add(item.text)
        }
        return msgs
    }

    fun clickAcuteMedications() {
        acuteMedications.toggleShrub()
    }

    fun clickCurrentRepeatMedications() {
        repeatMedications.toggleShrub()
    }

    fun clickDiscontinuedRepeatMedications() {
        discontinuedRepeatMedications.toggleShrub()
    }

    fun getAcuteMedicationsHeaderText(): String {
        return acuteMedications.header.element.text
    }

    fun getAcuteMedications(): String {
        return acuteMedications.firstElement.text
    }

    fun isAcuteMedicationsAvailable(): Boolean {
        return acuteMedications.firstElement.isPresent
    }

    fun isRepeatMedicationsAvailable(): Boolean {
        return repeatMedications.firstElement.isPresent
    }

    fun isDiscontinuedMedicationsAvailable(): Boolean {
        return discontinuedRepeatMedications.firstElement.isPresent
    }

    fun clickTestResultsSection() {
        testResults.toggleShrub()
    }

    fun getImmunistionsHeaderText(): String {
        return immunisations.header.element.text
    }

    fun getImmunisationRecordCount(): Int {
        return immunisations.allRecordItems().count()
    }

    fun clickImmunisations() {
        immunisations.toggleShrub()
    }

    fun getImmunisationsMessage(): String {
        return immunisations.firstParagraph.text
    }

    fun getTestResultsMessage(): String {
        return testResults.firstParagraph.text
    }

    fun getTestResultsHeaderText(): String {
        return testResults.header.element.text
    }

    fun getTestResultCount(): Int {
        return testResults.allRecordItems().size
    }

    fun getTestResultChildCount(): Int {
        return testResults.allRecordItems().get(0).findBy<WebElementFacade>(By.xpath("..")).thenFindAll(By.tagName("li")).size
    }

    fun isTestResultsTextMsgVisible(): Boolean {
        return testResults.firstParagraph.isCurrentlyVisible
    }

    fun getProblemsHeaderText(): String {
        return problems.header.element.text
    }

    fun getProblemsRecordCount(): Int {
        return problems.allRecordItemLabels().count()
    }

    fun clickProblems() {
        problems.toggleShrub()
    }

    fun getProblemsMessage(): String {
        return problems.firstParagraph.text
    }

    fun getConsultationsHeaderText(): String {
        return consultations.header.element.text
    }

    fun getConsultationsRecordCount(): Int {
        return consultations.allRecordItemLabels().count()
    }

    fun clickConsultations() {
        consultations.toggleShrub()
    }

    fun getConsultationsMessage(): String {
        return consultations.firstParagraph.text
    }

    fun getEventsHeaderText(): String {
        return eventsHeading.element.text
    }

    fun getEventsRecordCount(): Int {
        return events.element.thenFindAll(By.cssSelector("label")).count()
    }

    fun clickEvents() {
        toggleShrub(eventsHeading)
    }

    fun getEventsMessage(): String {
        return events.element.then<WebElementFacade>(By.cssSelector("p")).text
    }

    fun getSummaryCareNoAccessMessage(): String {
        return noSummaryCareAccessMessage.element.text
    }

    fun clickTestResult() {
        testResults.clickFirst()
    }

    private fun toggleShrub(shrub: HybridPageElement) {
        shrub.element.click()
        Thread.sleep(SHRUB_ANIMATION_DURATION_MILLIS)
    }
}
