package pages.navigation

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageElement
import pages.HybridPageObject

open class NavBar : HybridPageObject(Companion.PageType.NATIVE) {
    enum class NavBarType(val browserLocator: String, val androidLocator: String) {
        SYMPTOMS(
                "//nav//*[@data-sid='symptoms-menu-item']",
                "//android.widget.LinearLayout[contains(@resource-id, 'symptoms')]"),
        APPOINTMENTS(
                "//nav//*[@data-sid='appointments-menu-item']",
                "//android.widget.LinearLayout[contains(@resource-id, 'appointments')]"),
        PRESCRIPTIONS(
                "//nav//*[@data-sid='prescriptions-menu-item']",
                "//android.widget.LinearLayout[contains(@resource-id, 'prescriptions')]"),
        MY_RECORD(
                "//nav//*[@data-sid='myrecord-menu-item']",
                "//android.widget.LinearLayout[contains(@resource-id, 'myRecord')]"),
        MORE(
                "//nav//*[@data-sid='more-menu-item']",
                "//android.widget.LinearLayout[contains(@resource-id, 'more')]");
    }

    private fun getElement(element: NavBarType): WebElementFacade {
        return HybridPageElement(
                browserLocator = element.browserLocator,
                androidLocator = element.androidLocator,
                page = this)
                .element
    }

    fun select(type: NavBarType) {
        getElement(type).click()
    }

    fun isHighlighted(type: NavBarType): Boolean {
        return containsElements("${type.browserLocator}/ancestor::li[@class='active']")
    }

    fun hasAnActiveSelection() : Boolean {
        return containsElements( "//nav[descendant::li[@class='active']]")
    }

    fun isVisible(type: NavBarType): Boolean {
        return getElement(type).isVisible
    }
}