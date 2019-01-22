package pages.myrecord

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject

const val SHRUB_ANIMATION_DURATION_MILLIS: Long = 500

class MyRecordInfoPage : HybridPageObject() {

    val clinicalAbbreviationsLink =
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
                        .withText(expectedLabel, false)

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

    val examinations by lazy { getSection("Examinations") }

    val consultations by lazy { getSection("Consultations") }

    val diagnosis by lazy { getSection("Diagnosis") }
    
    val procedures by lazy { getSection("Procedures") }

    fun assertSectionHeaderIsVisible(header: String) {
        MyRecordWrapper(header, this).header.assertSingleElementPresent().assertIsVisible()
    }

    fun getSection(header: String): MyRecordWrapper {
        return MyRecordWrapper(header, this)
    }

    fun isNameVisible(): Boolean {
        return getValueFromField("Name").element.isCurrentlyVisible
    }

    fun clickClinicalAbbreviationsLink() {
        clinicalAbbreviationsLink.click()
        Thread.sleep(SHRUB_ANIMATION_DURATION_MILLIS)
    }

    fun getTestResultChildCount(): Int {
        return testResults.allRecordItems().first().element.findBy<WebElementFacade>(
                By.xpath("..")).thenFindAll(By.tagName("li")).size
    }

    fun isTestResultsTextMsgVisible(): Boolean {
        return testResults.firstParagraph.isCurrentlyVisible
    }

    fun isVisionTestResultsLinkVisible(): Boolean {
        return testResults.visionLink.element.isCurrentlyVisible
    }

    fun isDiagnosisTextMsgVisible(): Boolean {
        return diagnosis.firstParagraph.isCurrentlyVisible
    }

    fun isDiagnosisPageVisible(): Boolean {
        val viewDiagnosisRecordsLink =
                HybridPageElement(
                        browserLocator = "//a[contains(text(),'View your Diagnosis records')]",
                        androidLocator = null,
                        page = this)

        viewDiagnosisRecordsLink.click()

        Thread.sleep(SHRUB_ANIMATION_DURATION_MILLIS * 2)

        val diagnosisPageHeader =
                HybridPageElement(
                        browserLocator = "//h2[contains(text(),'Diagnosis')]",
                        androidLocator = null,
                        page = this)

        return diagnosisPageHeader.element.isVisible
    }

    fun isExaminationsTextMsgVisible(): Boolean {
        return examinations.firstParagraph.isCurrentlyVisible
    }

    fun getSummaryCareNoAccessMessage(): String {
        return noSummaryCareAccessMessage.element.text
    }
}
