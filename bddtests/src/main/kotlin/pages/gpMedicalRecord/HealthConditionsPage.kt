package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject

class HealthConditionsPage: HybridPageObject() {

    private val healthConditionsElements = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='health-conditions-card']",
        page = this
    )

    fun getHealthConditionsElements(): List<WebElementFacade> {
        return healthConditionsElements.elements
    }
}
