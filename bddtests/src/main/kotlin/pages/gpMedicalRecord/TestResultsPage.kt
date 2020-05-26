package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.By
import pages.HybridPageObject
import pages.myrecord.RecordItem

@DefaultUrl("http://web.local.bitraft.io:3000/health-records/test-results")
class TestResultsPage : HybridPageObject() {

    private val testResultsParentXpath = "//div[@data-purpose='record-item']"
    private val testResultXpath = "//a[contains(text(),'Pathology 1 - Anticoag Control (Warfarin), Read 1')]"

    val titleText: String = "Test results"

    fun getTestResultsElements(): List<RecordItem> {
        return findAllByXpath(testResultsParentXpath).map { element -> RecordItem(element) }
    }

    fun allRecordItems(webElement: List<WebElementFacade>): List<RecordItem> {
        return webElement.map { element -> RecordItem(element) }
    }

    fun getTestResultChildren(): List<WebElementFacade> {
        return getTestResultsElements().first().element.findBy<WebElementFacade>(
                By.xpath("..")).thenFindAll(By.tagName("li"))
    }

    fun clickTestResult() {
        findAllByXpath(testResultXpath).first().click()
    }

    fun WebElementFacade.findByXpath(xpath: String): WebElementFacade? {
        val elements = thenFindAll(xpath)
        return elements.firstOrNull()
    }
}
