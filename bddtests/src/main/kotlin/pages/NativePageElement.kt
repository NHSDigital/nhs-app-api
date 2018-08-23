package pages

import io.appium.java_client.MobileElement
import org.openqa.selenium.JavascriptExecutor
import org.openqa.selenium.NoSuchElementException

class NativePageElement(
        browserLocator: String,
        androidLocator: String? = null,
        iOSLocator: String? = null,
        private var iOSAccessID: String? = null,
        override var page: NativePageObject,
        helpfulName: String? = null
    ) : HybridPageElement(
        browserLocator,
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
                LOCATOR_STRATEGY_BROWSER -> page.findNativeByXpath(browserLocator).also {
                    if ((!it.isDisplayed).or(it.isUnderneathFixedElements())) {
                        it.scroll()
                    }
                }
                else -> throw IllegalArgumentException("Unknown element locator strategy.")
            }
        }

    private fun nativeLocatorStrategy(): String {
        return if (page.isIOS() && iOSAccessID != null) {
            LOCATOR_STRATEGY_IOS_ACCESSIBILITY
        } else {
            locatorStrategy();
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

    fun click() {
        if(page.onMobile())
            this.nativeElement.click()
        else
            this.element.click()

    }

    val text : String
        get() {
        if(page.isIOS())
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