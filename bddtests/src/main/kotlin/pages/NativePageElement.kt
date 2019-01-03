package pages

import io.appium.java_client.MobileElement
import org.openqa.selenium.JavascriptExecutor
import webdrivers.isIOS

class NativePageElement(
        webDesktopLocator: String,
        webMobileLocator: String,
        androidLocator: String? = null,
        iOSLocator: String? = null,
        private var iOSAccessID: String? = null,
        override var page: NativePageObject,
        helpfulName: String? = null
    ) : HybridPageElement(
        webDesktopLocator,
        webMobileLocator,
        androidLocator,
        iOSLocator,
        page as HybridPageObject,
        helpfulName
) {

    val nativeElement: MobileElement
        get() {
            return when (nativeLocatorStrategy()) {
                LOCATOR_STRATEGY_IOS -> page.findNativeByXpath(iOSLocator!!)
                LOCATOR_STRATEGY_IOS_ACCESSIBILITY -> page.findByAccessibilityId(iOSAccessID!!)
                LOCATOR_STRATEGY_ANDROID -> page.findNativeByXpath(androidLocator!!)
                LOCATOR_STRATEGY_WEBVIEW,
                LOCATOR_STRATEGY_BROWSER_MOBILE -> page.findNativeByXpath(webMobileLocator).also {
                    if ((!it.isDisplayed).or(it.isUnderneathFixedElements())) {
                        it.scroll()
                    }
                    val executor = (page.driver) as JavascriptExecutor
                    executor.executeScript("// TODO Invoke hidden mobile view navigation buttons when in native.")
                }
                LOCATOR_STRATEGY_BROWSER_DESKTOP -> page.findNativeByXpath(webDesktopLocator).also {
                    if ((!it.isDisplayed).or(it.isUnderneathFixedElements())) {
                        it.scroll()
                    }
                }
                else -> throw IllegalArgumentException("Unknown element locator strategy.")
            }
        }

    private fun nativeLocatorStrategy(): String {
        return if (page.driver.isIOS() && iOSAccessID != null) {
            LOCATOR_STRATEGY_IOS_ACCESSIBILITY
        } else {
            locatorStrategy()
        }
    }

    private fun MobileElement.isUnderneathFixedElements(): Boolean {

        val element = this

        val highestPixel = element.location.y
        val lowestPixel = highestPixel + element.size.height

        val pageLength = ((page.driver as JavascriptExecutor)
                .executeScript("return window.innerHeight || document.body.clientHeight", this) as Long)
                .toInt()

        val isBehindHeader = highestPixel < HEADER_HEIGHT_PX
        val isBehindFooter = lowestPixel > pageLength - (
                FLOATING_BUTTON_HEIGHT_PX + NAVBAR_HEIGHT_PX)

        return isBehindHeader.or(isBehindFooter)

    }

    override fun click() {
        if(page.onMobile()) {
            this.nativeElement.click()
        }
        else
            super.click()

    }

    val text : String
        get() {
        if(page.onMobile())
            return this.nativeElement.text

        return this.element.text
    }


    fun isDisplayed(): Boolean {
        return when(page.onMobile()) {
                true -> {this.nativeElement.isDisplayed }
                false -> {this.element.isDisplayed }
            }
    }

    private fun MobileElement.scroll() {
        scrollTo(this)
    }
}