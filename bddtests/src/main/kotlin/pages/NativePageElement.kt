package pages

import io.appium.java_client.MobileElement
import org.openqa.selenium.JavascriptExecutor
import org.openqa.selenium.NoSuchElementException
import org.openqa.selenium.StaleElementReferenceException
import org.openqa.selenium.WebDriverException
import org.openqa.selenium.WebElement
import webdrivers.getLocatorStrategy
import webdrivers.isIOS
import java.lang.AssertionError

const val MAX_WEB_DRIVER_EXCEPTIONS = 5
const val MAX_ASSERTION_ERRORS = 2

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
                LocatorStrategy.IOS -> page.findNativeByXpath(iOSLocator!!).also {
                    setHelpfulNameToUseFromLocator(iOSLocator!!)
                    it.getWrappedElementWithRetry()
                }
                LocatorStrategy.IOS_ACCESSIBILITY -> page.findByAccessibilityId(iOSAccessID!!).also {
                    setHelpfulNameToUseFromLocator(iOSAccessID!!)
                    it.getWrappedElementWithRetry()
                }
                LocatorStrategy.ANDROID -> page.findNativeByXpath(androidLocator!!).also {
                    setHelpfulNameToUseFromLocator(androidLocator!!)
                    it.getWrappedElementWithRetry()
                }
                LocatorStrategy.WEBVIEW,
                LocatorStrategy.BROWSER_MOBILE -> page.findNativeByXpath(webMobileLocator).also {
                    setHelpfulNameToUseFromLocator(webMobileLocator)
                    if ((!it.isDisplayed).or(it.isUnderneathFixedElements())) {
                        it.scroll()
                    }
                    val executor = (page.driver) as JavascriptExecutor
                    executor.executeScript("// TODO Invoke hidden mobile view navigation buttons when in native.")
                }
                LocatorStrategy.BROWSER_DESKTOP -> page.findNativeByXpath(webDesktopLocator).also {
                    setHelpfulNameToUseFromLocator(webDesktopLocator)
                    if ((!it.isDisplayed).or(it.isUnderneathFixedElements())) {
                        it.scroll()
                    }
                }
            }
        }

    private fun nativeLocatorStrategy(): LocatorStrategy {
        return if (page.driver.isIOS() && iOSAccessID != null) {
            LocatorStrategy.IOS_ACCESSIBILITY
        } else {
            page.driver.getLocatorStrategy(this)
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
        var assertionErrors = 0
        var webDriverExceptions = 0
        while (true) {
            try {
                actionOn(selectNativeElement())
                break
            }
            catch(e: StaleElementReferenceException){
                println("Stale element exception suppressed")
            }
            catch(e: AssertionError) {
                assertionErrors++
                if (assertionErrors == MAX_ASSERTION_ERRORS) {
                    throw e
                }
                println("Assertion error suppressed")
            }
            catch(e: WebDriverException) {
                webDriverExceptions++
                if (webDriverExceptions == MAX_WEB_DRIVER_EXCEPTIONS) {
                    throw e
                }
                println(e.toString())
            }
            println("Retrying")
            Thread.sleep(MILLISECONDS_IN_A_SECOND)
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
