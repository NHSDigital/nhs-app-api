package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject

class ImmunisationsPage: HybridPageObject() {

    private val immunisationsElements = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='immunisations-card']",
        page = this
    )

    fun getImmunisationsElements(): List<WebElementFacade> {
        return immunisationsElements.elements
    }
}
