package pages.gpMedicalRecord

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageObject

class AllergiesAndReactionsPage : HybridPageObject() {

    private val allergiesParentXpath = "//div[@data-purpose='allergies-and-reactions-card']"

    fun getAllergiesAndReactionsElements(): List<WebElementFacade> {
        return findAllByXpath(allergiesParentXpath)
    }

    fun WebElementFacade.findByXpath(xpath: String): WebElementFacade? {
        val elements = thenFindAll(xpath)
        return elements.firstOrNull()
    }
}
