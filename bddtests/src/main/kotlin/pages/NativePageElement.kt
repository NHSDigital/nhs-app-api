package pages

import io.appium.java_client.MobileElement
import org.openqa.selenium.JavascriptExecutor
import org.openqa.selenium.NoSuchElementException
import org.openqa.selenium.StaleElementReferenceException
import org.openqa.selenium.WebElement
import webdrivers.isIOS
import java.lang.AssertionError

class NativePageElement(
        webDesktopLocator: String,
        webMobileLocator: String = webDesktopLocator,
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

    private fun selectNativeElement() : MobileElement {
            return when (nativeLocatorStrategy()) {
                LOCATOR_STRATEGY_IOS -> page.findNativeByXpath(iOSLocator!!).also {
                    it.getWrappedElementWithRetry()
                }
                LOCATOR_STRATEGY_IOS_ACCESSIBILITY -> page.findByAccessibilityId(iOSAccessID!!).also {
                    it.getWrappedElementWithRetry()
                }
                LOCATOR_STRATEGY_ANDROID -> page.findNativeByXpath(androidLocator!!).also {
                    it.getWrappedElementWithRetry()
                }
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
            waitForNativeElementToBecomeVisible().actOnTheNativeElement {
                it.click()
            }
        }
        else
            super.click()
    }

    fun actOnTheNativeElement(actionOn: (elem: MobileElement) -> Unit) {
        var retryAssertionsOnce = 2
        while(retryAssertionsOnce>0) {
            try {
                actionOn(selectNativeElement())
                break
            }
            catch(e: StaleElementReferenceException){
                Thread.sleep(MILLISECONDS_IN_A_SECOND)
            }
            catch(e: AssertionError) {
                retryAssertionsOnce--
                Thread.sleep(MILLISECONDS_IN_A_SECOND)
            }
        }
    }

    val text : String
        get() {
            return when (page.onMobile()){
            true -> {
                var text = ""
                actOnTheNativeElement { text = it.text }
                text
                }
            false -> {
               super.textValue
            }
        }
    }


    fun isDisplayed(): Boolean {
        return when(page.onMobile()) {
                true -> {this.selectNativeElement().isDisplayed }
                false -> {this.isDisplayed }
            }
    }

    private fun MobileElement.scroll() {
        scrollTo(this)
    }

    private fun MobileElement.getWrappedElementWithRetry() : WebElement {
        var retryCount = (timeToWaitForElement / ELEMENT_RETRY_TIME).toInt()

        var wrappedElement: WebElement? = null

        while(retryCount>=0) {
            try {
                wrappedElement = this
                break
            } catch (exception: NoSuchElementException) {
                when(retryCount) {
                    0 -> throw exception
                    else -> {
                        println("Could not find element. RETRYING")
                        retryCount--
                        Thread.sleep((ELEMENT_RETRY_TIME * MILLISECONDS_IN_A_SECOND).toLong())
                    }
                }
            }
        }
        return wrappedElement!!
    }
}