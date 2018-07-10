package pages.myrecord

import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.PageObject
import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.By

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

    @FindBy(xpath = "//div[@id='mainDiv']//div[@id='mainDiv']//main//child::div[@class='msg error'][1]")
    lateinit var noSummaryCareAccessMessage: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Acute medications')]")
    lateinit var acuteMedicationsHeading: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Acute medications')]/following-sibling::div[1]")
    lateinit var acuteMedications: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Current repeat medications')]")
    lateinit var currentRepeatMedicationsHeading: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Current repeat medications')]/following-sibling::div[1]")
    lateinit var txtcurrentRepeatMedications: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Discontinued repeat medications')]")
    lateinit var discontinuedRepeatMedicationsHeading: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Discontinued repeat medications')]/following-sibling::div[1]")
    lateinit var txtdiscontinuedRepeatMedications: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Test results')]")
    lateinit var testResultsHeader: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Test results')]/following-sibling::div[1]")
    lateinit var txttestResultsMsg: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Immunisations')]")
    lateinit var immunisationsHeading: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Immunisations')]/following-sibling::div[1]")
    lateinit var immunisations: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Problems')]")
    lateinit var problemsHeading: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Problems')]/following-sibling::div[1]")
    lateinit var problems: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Events')]")
    lateinit var eventsHeading: WebElementFacade

    @FindBy(xpath = "//h5[contains(text(),'Events')]/following-sibling::div[1]")
    lateinit var events: WebElementFacade

    fun isNameVisible(): Boolean {
        waitABit(2000)
        return txtName.isCurrentlyVisible
    }

    fun isAllergiesTextMsgVisible(): Boolean {
        waitABit(2000)
        return txtAllergyMsg.findBy<WebElementFacade>(By.tagName("div")).isCurrentlyVisible
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
        evaluateJavascript("arguments[0].scrollIntoView(true);", allergiesAndAdverseReactionsHeader);
        allergiesAndAdverseReactionsHeader.click()
    }

    fun getAllergyMessage(): String {
        return txtAllergyMsg.text
    }

    fun getNoAllergyMessage(): String {
        return txtAllergyMsg.findBy<WebElementFacade>(By.tagName("p")).text
    }

    fun getNoAcuteMedicationMsg(): String {
        return acuteMedications.findBy<WebElementFacade>(By.tagName("p")).text
    }

    fun getNoCurrentRepeatMedicationMsg(): String {
        return txtcurrentRepeatMedications.findBy<WebElementFacade>(By.tagName("p")).text
    }

    fun getNoDiscontinuedRepeatMedicationMsg(): String {
        return txtdiscontinuedRepeatMedications.findBy<WebElementFacade>(By.tagName("p")).text
    }

    fun getAllergyCount(): Int {
        return txtAllergyMsg.thenFindAll(By.tagName("li")).size
    }

    fun getAllergyMessages(): ArrayList<String> {
        var msgs = ArrayList<String>()
        var list = txtAllergyMsg.thenFindAll(By.tagName("p"))
        for (item in list) {
            msgs.add(item.text)
        }
        return msgs
    }

    fun getAllergyDates(): ArrayList<String> {
        var msgs = ArrayList<String>()
        var list = txtAllergyMsg.thenFindAll(By.tagName("label"))
        for (item in list) {
            msgs.add(item.text)
        }
        return msgs
    }

    fun getTestResultsMessage(): String {
        evaluateJavascript("arguments[0].scrollIntoView(true);", txttestResultsMsg);
        return txttestResultsMsg.findBy<WebElementFacade>(By.tagName("p")).text
    }

    fun clickAcuteMedications() {
        evaluateJavascript("arguments[0].scrollIntoView(true);", acuteMedicationsHeading);
        acuteMedicationsHeading.click()
    }

    fun clickCurrentRepeatMedications() {
        evaluateJavascript("arguments[0].scrollIntoView(true);", currentRepeatMedicationsHeading);
        currentRepeatMedicationsHeading.click()
    }

    fun clickDiscontinuedRepeatMedications() {
        evaluateJavascript("arguments[0].scrollIntoView(true);", discontinuedRepeatMedicationsHeading);
        discontinuedRepeatMedicationsHeading.click()
    }

    fun getAcuteMedicationsHeaderText(): String {
        return acuteMedicationsHeading.text
    }

    fun getAcuteMedications(): String {
        return acuteMedications.findBy<WebElementFacade>(By.tagName("p")).text
    }

    fun isAcuteMedicationsAvailable(): Boolean {
        waitABit(1000)
        return acuteMedications.findBy<WebElementFacade>(By.tagName("li")).isPresent
    }

    fun isRepeatMedicationsAvailable(): Boolean {
        waitABit(1000)
        return txtcurrentRepeatMedications.findBy<WebElementFacade>(By.tagName("li")).isPresent
    }

    fun isDiscontinuedMedicationsAvailable(): Boolean {
        waitABit(1000)
        return txtdiscontinuedRepeatMedications.findBy<WebElementFacade>(By.tagName("li")).isPresent
    }

    fun clickTestResultsSection() {
        waitABit(2000)
        evaluateJavascript("arguments[0].scrollIntoView(true);", testResultsHeader);
        testResultsHeader.click()
    }

    fun getImmunistionsHeaderText(): String {
        return immunisationsHeading.text
    }

    fun getImmunisationRecordCount(): Int {
        return immunisations.thenFindAll(By.cssSelector("ul li")).count()
    }

    fun clickImmunisations() {
        evaluateJavascript("arguments[0].scrollIntoView(true);", immunisations)
        immunisationsHeading.click()
    }

    fun getImmunisationsMessage(): String {
        return immunisations.then<WebElementFacade>(By.cssSelector("p")).text;
    }

    fun getTestResultsHeaderText(): String {
        return testResultsHeader.text
    }

    fun getTestResultCount(): Int {
        return txttestResultsMsg.thenFindAll(By.tagName("p")).size
    }

    fun getTestResultChildCount(): Int {
        return txttestResultsMsg.thenFindAll(By.tagName("p")).get(0).findBy<WebElementFacade>(By.xpath("..")).thenFindAll(By.tagName("li")).size
    }

    fun isTestResultsTextMsgVisible(): Boolean {
        waitABit(2000)
        return txttestResultsMsg.findBy<WebElementFacade>(By.tagName("div")).isCurrentlyVisible
    }

    fun getProblemsHeaderText(): String {
        return problemsHeading.text
    }

    fun getProblemsRecordCount(): Int {
        return problems.thenFindAll(By.cssSelector("label")).count()
    }

    fun clickProblems() {
        waitABit(2000)
        evaluateJavascript("arguments[0].scrollIntoView(true);", problems)
        problemsHeading.click()
    }

    fun getProblemsMessage(): String {
        return problems.then<WebElementFacade>(By.cssSelector("p")).text;
    }

    fun getEventsHeaderText(): String {
        return eventsHeading.text
    }

    fun getEventsRecordCount(): Int {
        return events.thenFindAll(By.cssSelector("label")).count()
    }

    fun clickEvents() {
        waitABit(2000)
        evaluateJavascript("arguments[0].scrollIntoView(true);", events)
        eventsHeading.click()
    }

    fun getEventsMessage(): String {
        return events.then<WebElementFacade>(By.cssSelector("p")).text;
    }


    fun getSummaryCareNoAccessMessage(): String {
        return noSummaryCareAccessMessage.text
    }
}
