package pages.navigation

import io.appium.java_client.MobileElement
import net.serenitybdd.core.pages.WebElementFacade
import pages.NativePageElement
import pages.NativePageObject

open class NavBarNative : NativePageObject() {

    enum class NavBarType(val browserLocator: String, val androidLocator: String?, val iOSAccessID: String?) {
        SYMPTOMS(
                "symptoms-menu-item",
                "symptoms",
                "Symptoms"),
        APPOINTMENTS(
                "appointments-menu-item",
                "appointments",
                "Appointments"),
        PRESCRIPTIONS(
                "prescriptions-menu-item",
                "prescriptions",
                "Prescriptions"),
        MY_RECORD(
                "myrecord-menu-item",
                "myRecord",
                "My record"),
        MORE(
                "more-menu-item",
                "more",
                "More")
    }

    private fun getNativePageElement(element: NavBarType): NativePageElement {
        if(onMobile())
            switchNative()

        return NativePageElement(
                browserLocator = "//nav//*[@data-sid='${element.browserLocator}']",
                androidLocator = "//*[contains(@resource-id, '${element.androidLocator}')]",
                iOSAccessID = element.iOSAccessID,
                page = this)
    }
    private fun getNativeElement(element: NavBarType): MobileElement {
        return getNativePageElement(element).nativeElement
    }

    private fun getElement(element: NavBarType): WebElementFacade {
        return getNativePageElement(element).element
    }

    private val selectedNavElements = NativePageElement(
            browserLocator = "//nav[descendant::li[@data-selected='true']]",
            androidLocator = "//*[contains(@content-desc,'selected')]",
            iOSAccessID = null,
            helpfulName = null,
            page = this
    )

    fun select(type: NavBarType) {
        getNativePageElement(type).click()
    }

    fun isHighlighted(type: NavBarType): Boolean {
        return when (onMobile()) {
            true -> {
                when (isAndroid()) {
                    true -> {
                        getNativeElement(type).findElementsByXPath("//*[contains(@content-desc,'selected')]").count()==1
                    }
                    false -> {
                        val navBarElements = getNativeElement(type)
                        ("1".equals(navBarElements.getAttribute("value")))
                                || ("More".equals(navBarElements.getAttribute("value")))
                    }
                }
            }
            false -> {
                containsElements(
                        "${getNativePageElement(type).browserLocator}/ancestor::li[@data-selected='true']") }
        }
    }

    fun hasSingleSelection(): Boolean {
        var highlightedCount = 0
        if(isIOS()){
            val navBar = arrayListOf(
                    NavBarNative.NavBarType.SYMPTOMS,
                    NavBarNative.NavBarType.APPOINTMENTS,
                    NavBarNative.NavBarType.PRESCRIPTIONS,
                    NavBarNative.NavBarType.MORE,
                    NavBarNative.NavBarType.MY_RECORD
            )
            for (eachElement in navBar) {
                if(isHighlighted(eachElement))
                    highlightedCount++
            }
            return highlightedCount == 1
        } else {
            return selectedNavElements.elements.size == 1
        }
    }
    
    fun hasAnActiveSelection() : Boolean {
        return containsElements( "//*/ancestor::li[@data-selected='true']")
    }

    fun isVisible(type: NavBarType): Boolean {
        if(onMobile())
            return getNativeElement(type).isDisplayed

        return getElement(type).isDisplayed
    }
}