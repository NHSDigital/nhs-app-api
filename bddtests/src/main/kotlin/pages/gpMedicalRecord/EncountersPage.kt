package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject

class EncountersPage: HybridPageObject() {

    private val encountersElements = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='encounters-card']",
        page = this
    )

    fun getEncountersElements(): List<WebElementFacade> {
        return encountersElements.elements
    }
}
