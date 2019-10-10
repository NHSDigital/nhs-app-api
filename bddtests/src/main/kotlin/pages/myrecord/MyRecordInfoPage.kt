package pages.myrecord

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.NoSuchElementException
import pages.HybridPageObject
import pages.HybridPageElement
import pages.assertSingleElementPresent
import pages.text
import pages.assertIsVisible
import pages.assertElementNotPresent
import pages.isCurrentlyVisible
import pages.isVisible

const val SHRUB_ANIMATION_DURATION_MILLIS: Long = 500


class MyRecordInfoPage : HybridPageObject() {

    val clinicalAbbreviationsLink =
            HybridPageElement(
                    webDesktopLocator = "//a[contains(text(),'Help with abbreviations')]",
                    androidLocator = null,
                    page = this)

    private val noRecordsOrNoAccessText = "Sorry, this information isn't available through the NHS App. " +
            "To access it, contact your GP surgery."
    val noRecordsOrNoAccessParagraph =
            HybridPageElement(
                    webDesktopLocator = "//strong[contains(text(), \"${noRecordsOrNoAccessText}\")]",
                    androidLocator = null,
                    page = this)

    private val noSummaryCareAccessMessage =
            HybridPageElement(
                    webDesktopLocator = "//div[@id='mainDiv']//div[@id='errorMsg']",
                    androidLocator = null,
                    page = this)

    fun assertLabelAndValue(expectedLabel: String, expectedValue: String) {
        val labelElement =
                HybridPageElement(
                        webDesktopLocator = "//p[@data-purpose='record-item-header']",
                        androidLocator = null,
                        page = this,
                        helpfulName = "Label for '$expectedLabel'")
                        .withText(expectedLabel, false)

        labelElement.assertSingleElementPresent()
        val value = getValueFromField(expectedLabel)

        Assert.assertEquals("Value for $expectedLabel",
                expectedValue,
                value.assertSingleElementPresent().text)
    }

    private fun getValueFromField(label: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "//p${String.format(containsTextXpathSubstring, label)}/following-sibling::p[1]",
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

    val medicalHistories by lazy { getSection("Medical history") }

    val healthConditions by lazy { getSection("Health conditions") }

    val examinations by lazy { getSection("Examinations") }

    val consultations by lazy { getSection("Consultations") }

    val diagnosis by lazy { getSection("Diagnosis") }

    val procedures by lazy { getSection("Procedures") }

    val recalls by lazy { getSection("Recalls") }

    val documents by lazy { getSection("Documents") }

    val encounters by lazy { getSection("Encounters") }

    val referrals by lazy { getSection("Referrals") }

    fun assertSectionHeaderIsVisible(header: String) {
        MyRecordWrapper(header, this).header.assertSingleElementPresent().assertIsVisible()
    }

    fun assertSectionHeaderNotPresent(header: String) {
        MyRecordWrapper(header, this).header.assertElementNotPresent()
    }

    fun getSection(header: String): MyRecordWrapper {
        return MyRecordWrapper(header, this)
    }

    fun assertSectionCollapsed(heading: String){
        getSection(heading).firstParagraph.assertElementNotPresent()
    }

    fun isNameVisible(): Boolean {
        return try {
                getValueFromField("Name").isCurrentlyVisible
            } catch (e: NoSuchElementException) {
                false
            }

    }

    fun getTestResultChildCount(): Int {
        return testResults.allRecordItems().first().element.findBy<WebElementFacade>(
                By.xpath("..")).thenFindAll(By.tagName("li")).size
    }

    fun isTestResultsTextMsgVisible(): Boolean {
        return testResults.firstParagraph.isCurrentlyVisible
    }

    fun isVisionTestResultsLinkVisible(): Boolean {
        return testResults.visionLink.isCurrentlyVisible
    }

    fun isVisionSectionPageVisible(linkText: String, sectionName: String): Boolean {
        val sectionLink =
                HybridPageElement(
                        webDesktopLocator = "//a[contains(text(),'View your $linkText records')]",
                        androidLocator = null,
                        page = this)

        sectionLink.click()

        Thread.sleep(SHRUB_ANIMATION_DURATION_MILLIS * 2)

        val sectionPageHeader =
                HybridPageElement(
                        webDesktopLocator = "//h2[contains(text(),'$sectionName')]",
                        androidLocator = null,
                        page = this)

        return sectionPageHeader.isVisible
    }

    fun getSummaryCareNoAccessMessage(): String {
        return noSummaryCareAccessMessage.text
    }
}
