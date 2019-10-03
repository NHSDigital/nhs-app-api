package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject
import pages.myrecord.RecordItem

class ImmunisationsPage: HybridPageObject() {

    val immunisationsElements = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='immunisations-card']",
        page = this
    )

    fun getImmunisationsElements(): List<WebElementFacade> {
        return immunisationsElements.elements
    }

    fun allRecordItems(): List<RecordItem> {
        return getImmunisationsElements().map { element -> RecordItem(element) }
    }
}
