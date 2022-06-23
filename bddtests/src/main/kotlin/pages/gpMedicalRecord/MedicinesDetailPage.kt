package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject

class MedicinesDetailPage: HybridPageObject() {

    private val medicinesElements = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='medicines-card']",
        page = this
    )

    fun getMedicinesElements(): List<WebElementFacade> {
        return medicinesElements.elements
    }
}
