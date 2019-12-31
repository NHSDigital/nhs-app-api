package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject

class RecallsPage: HybridPageObject() {

    private val recallElements = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='recalls-card']",
        page = this
    )

    fun getRecallElements(): List<WebElementFacade> {
        return recallElements.elements
    }
}
