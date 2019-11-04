package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject
import pages.myrecord.RecordItem

class EventsPage: HybridPageObject() {

    val eventsElements = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='events-card']",
        page = this
    )

    fun getEventsElements(): List<WebElementFacade> {
        return eventsElements.elements
    }

    fun allRecordItems(): List<RecordItem> {
        return getEventsElements().map { element -> RecordItem(element) }
    }
}
