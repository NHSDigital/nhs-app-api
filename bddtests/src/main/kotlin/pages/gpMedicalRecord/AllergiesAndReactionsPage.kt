package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageObject
import pages.myrecord.RecordItem

class AllergiesAndReactionsPage : HybridPageObject() {

    private val allergiesParentXpath = "//div[@data-purpose='allergies-and-reactions-card']"

    fun getAllergiesAndReactionsElements(): List<WebElementFacade> {
        return findAllByXpath(allergiesParentXpath)
    }

    fun allRecordItems(): List<RecordItem> {
        return getAllergiesAndReactionsElements().map { element -> RecordItem(element) }
    }

    fun WebElementFacade.findByXpath(xpath: String): WebElementFacade? {
        val elements = thenFindAll(xpath)
        return elements.firstOrNull()
    }
}
