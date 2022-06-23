package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject

class ConsultationsPage: HybridPageObject() {

    private val consultationsElements = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='consultations-card']",
        page = this
    )

    fun getConsultationsElements(): List<WebElementFacade> {
        return consultationsElements.elements
    }
}
