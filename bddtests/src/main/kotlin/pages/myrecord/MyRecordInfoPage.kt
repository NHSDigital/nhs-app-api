package pages.myrecord

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject

const val SHRUB_ANIMATION_DURATION_MILLIS: Long = 500

@Suppress("TooManyFunctions")
class MyRecordInfoPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    private val clinicalAbbreviationsLink =
            HybridPageElement(
                    browserLocator = "//a[contains(text(),'Help with abbreviations')]",
                    androidLocator = null,
                    page = this)

    private val noSummaryCareAccessMessage =
            HybridPageElement(
                    browserLocator = "//div[@id='mainDiv']//div[@id='errorMsg']",
                    androidLocator = null,
                    page = this)

    fun assertLabelAndValue(expectedLabel: String, expectedValue: String) {
        val labelElement =
                HybridPageElement(
                        browserLocator = "//span",
                        androidLocator = null,
                        page = this,
                        helpfulName = "Label for '$expectedLabel'")
                        .containingText(expectedLabel)

        labelElement.assertSingleElementPresent()
        val value = getValueFromField(expectedLabel)

        Assert.assertEquals("Value for $expectedLabel",
                expectedValue,
                value.assertSingleElementPresent().element.text)
    }

    private fun getValueFromField(label: String): HybridPageElement {
        return HybridPageElement(
                browserLocator = "//span${String.format(containsTextXpathSubstring, label)}/following-sibling::p[1]",
                androidLocator = null,
                page = this,
                helpfulName = "Label for '$label'")
    }

    val myDetails by lazy { getSection("My details") }

    val allergies by lazy { getSection("Allergies and adverse reactions") }

    val acuteMedications by lazy { getSection("Acute (short-term) medications") }

    val repeatMedications by lazy { getSection("Repeat medications: current") }

    val discontinuedRepeatMedications by lazy { getSection("Repeat medications: discontinued") }

    val testResults by lazy { getSection("Test results") }

    val immunisations by lazy { getSection("Immunisations") }

    val problems by lazy { getSection("Problems") }

    val consultations by lazy { getSection("Consultations") }


    fun assertSectionHeaderIsVisible(header: String) {
        MyRecordWrapper(header, this).header.assertSingleElementPresent().assertIsVisible()
    }

    fun getSection(header: String): MyRecordWrapper {
        return MyRecordWrapper(header, this)
    }

    fun isNameVisible(): Boolean {
        return getValueFromField("Name").element.isCurrentlyVisible
    }

    fun canSeeClinicalAbbreviationsLink(): Boolean {
        return clinicalAbbreviationsLink.element.isPresent
    }

    fun clickClinicalAbbreviationsLink() {
        toggleShrub(clinicalAbbreviationsLink)
    }

    fun getAllergyCount(): Int {
        return allergies.allRecordItems().count()
    }

    fun getAllergyMessages(): List<String> {
        return allergies.allRecordItemBodies().map { item -> item.text }
    }

    fun getAllergyDates(): List<String> {
        return allergies.allRecordItemLabels().map { msg -> msg.text }
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

    fun getImmunisationRecordCount(): Int {
        return immunisations.allRecordItems().count()
    }

    fun getTestResultCount(): Int {
        return testResults.allRecordItems().size
    }

    fun getTestResultChildCount(): Int {
        return testResults.allRecordItems().get(0).findBy<WebElementFacade>(
                By.xpath("..")).thenFindAll(By.tagName("li")).size
    }

    fun isTestResultsTextMsgVisible(): Boolean {
        return testResults.firstParagraph.isCurrentlyVisible
    }

    fun getProblemsRecordCount(): Int {
        return problems.allRecordItemLabels().count()
    }

    fun getConsultationsRecordCount(): Int {
        return consultations.allRecordItemLabels().count()
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
