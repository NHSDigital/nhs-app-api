package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject
import pages.myrecord.RecordItem

class MedicinesDetailPage: HybridPageObject() {

    val medicinesElements = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='medicines-card']",
        page = this
    )

    fun getMedicinesElements(): List<WebElementFacade> {
        return medicinesElements.elements
    }

    fun allRecordItems(): List<RecordItem> {
        return getMedicinesElements().map { element -> RecordItem(element) }
    }
}
