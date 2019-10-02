package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.By
import pages.HybridPageObject
import pages.myrecord.RecordItem

@DefaultUrl("http://web.local.bitraft.io:3000/gp-medical-record/test-results")
class TestResultsPage : HybridPageObject() {

    private val testResultsParentXpath = "//div[@data-purpose='record-item']"

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

    fun WebElementFacade.findByXpath(xpath: String): WebElementFacade? {
        val elements = thenFindAll(xpath)
        return elements.firstOrNull()
    }
}
