package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject

class EventsPage: HybridPageObject() {

    private val eventsElements = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='events-card']",
        page = this
    )

    fun getEventsElements(): List<WebElementFacade> {
        return eventsElements.elements
    }
}
