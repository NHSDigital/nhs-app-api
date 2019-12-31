package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject

class MedicalHistoryPage: HybridPageObject() {

    private val historyElements = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='medical-history-card']",
        page = this
    )

    fun getMedicalHistoryElements(): List<WebElementFacade> {
        return historyElements.elements
    }
}
