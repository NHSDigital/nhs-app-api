package pages.navigation

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageObject

open class NavBar : HybridPageObject(Companion.PageType.NATIVE) {
    enum class NavBarType(val locator: String) {
        SYMPTOMS("//span[contains(text(),'Symptoms')]"),
        APPOINTMENTS("//span[contains(text(),'Appointments')]"),
        PRESCRIPTIONS("//span[contains(text(),'Prescriptions')]"),
        MY_RECORD("//span[contains(text(),'My record')]"),
        MORE("//span[contains(text(),'More')]");
    }

    fun select(type: NavBarType) {
        findBy<WebElementFacade>(type.locator)
                .waitUntilVisible<WebElementFacade>()
                .click()
    }

    fun isHighlighted(type: NavBarType): Boolean {
        return containsElements("${type.locator}/ancestor::li[@class='active']")
    }

    fun isVisible(type: NavBarType): Boolean {
        return findBy<WebElementFacade>("${type.locator}").isVisible
    }
}