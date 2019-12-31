package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject
import pages.myrecord.RecordItem

class ConsultationsPage: HybridPageObject() {

    private val consultationsElements = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='consultations-card']",
        page = this
    )

    fun getConsultationsElements(): List<WebElementFacade> {
        return consultationsElements.elements
    }

    fun allRecordItems(): List<RecordItem> {
        return getConsultationsElements().map { element -> RecordItem(element) }
    }
}
