package pages.navigation

import io.appium.java_client.MobileElement
import net.serenitybdd.core.pages.WebElementFacade
import pages.NativePageElement
import pages.NativePageObject
import webdrivers.isAndroid
import webdrivers.options.OptionManager
import webdrivers.options.device.DeviceWebMobile
import webdrivers.options.nojs.NoJsOption

open class NavBarNative : NativePageObject() {

    enum class NavBarType(val webDesktopLocator: String,
                          val webMobileLocator: String,
                          val androidLocator: String?,
                          val iOSAccessID: String?,
                          val nativeOnly: Boolean = false) {
        SYMPTOMS(
                "symptoms-menu-item",
                "symptoms-menu-item",
                "symptoms",
                "Symptoms"),
        APPOINTMENTS(
                "appointments-menu-item",
                "appointments-menu-item",
                "appointments",
                "Appointments"),
        PRESCRIPTIONS(
                "prescriptions-menu-item",
                "prescriptions-menu-item",
                "prescriptions",
                "Prescriptions"),
        MY_RECORD(
                "myrecord-menu-item",
                "myrecord-menu-item",
                "myRecord",
                "My record"),
        MORE(
                "more-menu-item",
                "more-menu-item",
                "more",
                "More",
                true)
    }

    private fun getNativePageElement(element: NavBarType): NativePageElement {
        if (onMobile())
            switchNative()

        return NativePageElement(
                webDesktopLocator = "//nav//*[@data-sid='${element.webDesktopLocator}']",
                webMobileLocator = "//nav//*[@data-sid='${element.webMobileLocator}']",
                androidLocator = "//*[contains(@resource-id, '${element.androidLocator}')]",
                iOSAccessID = element.iOSAccessID,
                page = this)
    }

    private fun getNativeElement(element: NavBarType): MobileElement {
        return getNativePageElement(element).nativeElement
    }

    private fun getElement(element: NavBarType): WebElementFacade {
        var webElement: WebElementFacade? = null
        getNativePageElement(element).actOnTheElement { webElement = it }
        return webElement!!
    }

    fun select(type: NavBarType) {
        initailiseMenu()
        getNativePageElement(type).click()
    }

    fun initailiseMenu() {
        return when {
            OptionManager.instance().isEnabled(DeviceWebMobile::class)
                    && !OptionManager.instance().isEnabled(NoJsOption::class) ->
                this.findByXpath("//a[@data-sid='mini-menu']").click()
            else -> {
            }
        }
    }


    fun isHighlighted(type: NavBarType): Boolean {
        return when (onMobile()) {
            true -> {
                when (driver.isAndroid()) {
                    true -> {
                        getNativeElement(type).
                                findElementsByXPath("//*[contains(@content-desc,'selected')]").
                                count() == 1
                    }
                    false -> {
                        val navBarElements = getNativeElement(type)
                        ("1".equals(navBarElements.getAttribute("value")))
                                || ("More".equals(navBarElements.getAttribute("value")))
                    }
                }
            }
            false -> true
        }
    }

    fun hasSingleSelection(): Boolean {
        var highlightedCount = 0
        if (onMobile()) {
            val navBar = arrayListOf(
                    NavBarNative.NavBarType.SYMPTOMS,
                    NavBarNative.NavBarType.APPOINTMENTS,
                    NavBarNative.NavBarType.PRESCRIPTIONS,
                    NavBarNative.NavBarType.MORE,
                    NavBarNative.NavBarType.MY_RECORD
            )
            for (eachElement in navBar) {
                if (isHighlighted(eachElement))
                    highlightedCount++
            }
            return highlightedCount == 1
        } else {
            return true // selectedNavElements.elements.size == 1
        }
    }

    fun isVisible(type: NavBarType): Boolean {
        if (onMobile())
            return getNativeElement(type).isDisplayed

        return getElement(type).isDisplayed
    }
}
