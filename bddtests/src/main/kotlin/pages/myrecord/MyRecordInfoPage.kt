package pages.myrecord

import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject

class MyRecordInfoPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val secMyDetails = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'My details')]",
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

    val allergiesAndAdverseReactionsHeader = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Allergies and adverse reactions')]",
            androidLocator = null,
            page = this)

    val txtAllergyMsg = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Allergies and adverse reactions')]/following-sibling::div[1]",
            androidLocator = null,
            page = this)

    val noSummaryCareAccessMessage = 
        HybridPageElement(
            browserLocator = "//div[@id='mainDiv']//div[@id='mainDiv']//main//child::div[@class='msg error'][1]",
            androidLocator = null,
            page = this)

    val acuteMedicationsHeading = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Acute medications')]",
            androidLocator = null,
            page = this)

    val acuteMedications = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Acute medications')]/following-sibling::div[1]",
            androidLocator = null,
            page = this)

    val currentRepeatMedicationsHeading = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Current repeat medications')]",
            androidLocator = null,
            page = this)

    val txtcurrentRepeatMedications = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Current repeat medications')]/following-sibling::div[1]",
            androidLocator = null,
            page = this)

    val discontinuedRepeatMedicationsHeading = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Discontinued repeat medications')]",
            androidLocator = null,
            page = this)

    val txtdiscontinuedRepeatMedications = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Discontinued repeat medications')]/following-sibling::div[1]",
            androidLocator = null,
            page = this)

    val testResultsHeader = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Test results')]",
            androidLocator = null,
            page = this)

    val txttestResultsMsg = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Test results')]/following-sibling::div[1]",
            androidLocator = null,
            page = this)

    val immunisationsHeading = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Immunisations')]",
            androidLocator = null,
            page = this)

    val immunisations = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Immunisations')]/following-sibling::div[1]",
            androidLocator = null,
            page = this)

    val problemsHeading = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Problems')]",
            androidLocator = null,
            page = this)

    val problems = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Problems')]/following-sibling::div[1]",
            androidLocator = null,
            page = this)

    val consultationsHeading = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Consultations')]",
            androidLocator = null,
            page = this)

    val consultations = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Consultations')]/following-sibling::div[1]",
            androidLocator = null,
            page = this)

    val eventsHeading = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Events')]",
            androidLocator = null,
            page = this)

    val events = 
        HybridPageElement(
            browserLocator = "//h5[contains(text(),'Events')]/following-sibling::div[1]",
            androidLocator = null,
            page = this)

    fun isNameVisible(): Boolean {
        return txtName.element.isCurrentlyVisible
    }

    fun isAllergiesTextMsgVisible(): Boolean {
        return txtAllergyMsg.element.findBy<WebElementFacade>(By.tagName("div")).isCurrentlyVisible
    }

    fun isOnMyRecordInfoPage(): Boolean {
        return secMyDetails.element.isPresent
    }

    fun getMyDetailsLabelText(): String {
        return secMyDetails.element.text
    }

    fun clickMyDetails() {
        secMyDetails.element.click()
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
        return allergiesAndAdverseReactionsHeader.element.text
    }

    fun clickAllergiesAndAdverseReactionsSection() {
        allergiesAndAdverseReactionsHeader.element.click()
    }

    fun getAllergyMessage(): String {
        return txtAllergyMsg.element.text
    }

    fun getNoAllergyMessage(): String {
        return txtAllergyMsg.element.findBy<WebElementFacade>(By.tagName("p")).text
    }

    fun getNoAcuteMedicationMsg(): String {
        return acuteMedications.element.findBy<WebElementFacade>(By.tagName("p")).text
    }

    fun getNoCurrentRepeatMedicationMsg(): String {
        return txtcurrentRepeatMedications.element.findBy<WebElementFacade>(By.tagName("p")).text
    }

    fun getNoDiscontinuedRepeatMedicationMsg(): String {
        return txtdiscontinuedRepeatMedications.element.findBy<WebElementFacade>(By.tagName("p")).text
    }

    fun getAllergyCount(): Int {
        return txtAllergyMsg.element.thenFindAll(By.tagName("li")).size
    }

    fun getAllergyMessages(): ArrayList<String> {
        val msgs = ArrayList<String>()
        val list = txtAllergyMsg.element.thenFindAll(By.tagName("p"))
        for (item in list) {
            msgs.add(item.text)
        }
        return msgs
    }

    fun getAllergyDates(): ArrayList<String> {
        val msgs = ArrayList<String>()
        val list = txtAllergyMsg.element.thenFindAll(By.tagName("label"))
        for (item in list) {
            msgs.add(item.text)
        }
        return msgs
    }

    fun getTestResultsMessage(): String {
        return txttestResultsMsg.element.findBy<WebElementFacade>(By.tagName("p")).text
    }

    fun clickAcuteMedications() {
        acuteMedicationsHeading.element.click()
    }

    fun clickCurrentRepeatMedications() {
        currentRepeatMedicationsHeading.element.click()
    }

    fun clickDiscontinuedRepeatMedications() {
        discontinuedRepeatMedicationsHeading.element.click()
    }

    fun getAcuteMedicationsHeaderText(): String {
        return acuteMedicationsHeading.element.text
    }

    fun getAcuteMedications(): String {
        return acuteMedications.element.findBy<WebElementFacade>(By.tagName("p")).text
    }

    fun isAcuteMedicationsAvailable(): Boolean {
        return acuteMedications.element.findBy<WebElementFacade>(By.tagName("li")).isPresent
    }

    fun isRepeatMedicationsAvailable(): Boolean {
        return txtcurrentRepeatMedications.element.findBy<WebElementFacade>(By.tagName("li")).isPresent
    }

    fun isDiscontinuedMedicationsAvailable(): Boolean {
        return txtdiscontinuedRepeatMedications.element.findBy<WebElementFacade>(By.tagName("li")).isPresent
    }

    fun clickTestResultsSection() {
        testResultsHeader.element.click()
    }

    fun getImmunistionsHeaderText(): String {
        return immunisationsHeading.element.text
    }

    fun getImmunisationRecordCount(): Int {
        return immunisations.element.thenFindAll(By.cssSelector("ul li")).count()
    }

    fun clickImmunisations() {
        immunisationsHeading.element.click()
    }

    fun getImmunisationsMessage(): String {
        return immunisations.element.then<WebElementFacade>(By.cssSelector("p")).text
    }

    fun getTestResultsHeaderText(): String {
        return testResultsHeader.element.text
    }

    fun getTestResultCount(): Int {
        return txttestResultsMsg.element.thenFindAll(By.tagName("p")).size
    }

    fun getTestResultChildCount(): Int {
        return txttestResultsMsg.element.thenFindAll(By.tagName("p")).get(0).findBy<WebElementFacade>(By.xpath("..")).thenFindAll(By.tagName("li")).size
    }

    fun isTestResultsTextMsgVisible(): Boolean {
        return txttestResultsMsg.element.findBy<WebElementFacade>(By.tagName("div")).isCurrentlyVisible
    }

    fun getProblemsHeaderText(): String {
        return problemsHeading.element.text
    }

    fun getProblemsRecordCount(): Int {
        return problems.element.thenFindAll(By.cssSelector("label")).count()
    }

    fun clickProblems() {
        problemsHeading.element.click()
    }

    fun getProblemsMessage(): String {
        return problems.element.then<WebElementFacade>(By.cssSelector("p")).text
    }

    fun getConsultationsHeaderText(): String {
        return consultationsHeading.element.text
    }

    fun getConsultationsRecordCount(): Int {
        return consultations.element.thenFindAll(By.cssSelector("label")).count()
    }

    fun clickConsultations() {
        consultationsHeading.element.click()
    }

    fun getConsultationsMessage(): String {
        return consultations.element.then<WebElementFacade>(By.cssSelector("p")).text
    }

    fun getEventsHeaderText(): String {
        return eventsHeading.element.text
    }

    fun getEventsRecordCount(): Int {
        return events.element.thenFindAll(By.cssSelector("label")).count()
    }

    fun clickEvents() {
        eventsHeading.element.click()
    }

    fun getEventsMessage(): String {
        return events.element.then<WebElementFacade>(By.cssSelector("p")).text
    }

    fun getSummaryCareNoAccessMessage(): String {
        return noSummaryCareAccessMessage.element.text
    }
}
