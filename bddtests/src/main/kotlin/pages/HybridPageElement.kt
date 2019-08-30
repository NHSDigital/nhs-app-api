package pages

import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.JavascriptExecutor
import org.openqa.selenium.Keys
import org.openqa.selenium.NoSuchElementException
import org.openqa.selenium.StaleElementReferenceException
import org.openqa.selenium.WebElement
import webdrivers.isAndroid
import webdrivers.isIOS
import webdrivers.options.OptionManager
import webdrivers.options.device.DeviceWebMobile
import java.lang.AssertionError

const val LOCATOR_STRATEGY_ANDROID = "ANDROID"
const val LOCATOR_STRATEGY_WEBVIEW = "WEBVIEW"
const val LOCATOR_STRATEGY_BROWSER_DESKTOP = "BROWSER_DESKTOP"
const val LOCATOR_STRATEGY_BROWSER_MOBILE = "BROWSER_MOBILE"
const val LOCATOR_STRATEGY_IOS = "IOS"
const val LOCATOR_STRATEGY_IOS_ACCESSIBILITY = "IOSACCESS"

const val HEADER_HEIGHT_PX = 100
const val FLOATING_BUTTON_HEIGHT_PX = 78.5
const val NAVBAR_HEIGHT_PX = 70

private const val DEFAULT_WAIT_TIME = 500L
private const val DEFAULT_RETRIES = 3
const val TIME_TO_WAIT_FOR_ELEMENT = 10
const val ELEMENT_RETRY_TIME = 1.0
const val MILLISECONDS_IN_A_SECOND = 1000L

open class HybridPageElement(
        var webDesktopLocator: String,
        var webMobileLocator: String = webDesktopLocator,
        var androidLocator: String? = null,
        var iOSLocator: String? = null,
        open val page: HybridPageObject,
        helpfulName: String? = null,
        var timeToWaitForElement: Int = TIME_TO_WAIT_FOR_ELEMENT
) {
    val helpfulNameToUse = helpfulName ?: webDesktopLocator

    private fun selectElement() : WebElementFacade {
        return when (locatorStrategy()) {
            LOCATOR_STRATEGY_IOS -> page.findByXpath(iOSLocator!!).also {
                it.getWrappedElementWithRetry()
            }
            LOCATOR_STRATEGY_ANDROID -> page.findByXpath(androidLocator!!).also {
                it.getWrappedElementWithRetry()
            }
            LOCATOR_STRATEGY_WEBVIEW,
            LOCATOR_STRATEGY_BROWSER_DESKTOP -> page.findByXpath(webDesktopLocator).also {
                if ((it.isUnderneathFixedElements()).or(!it.isCurrentlyVisible)) {
                    scrollTo(it)
                }
            }
            LOCATOR_STRATEGY_BROWSER_MOBILE -> page.findByXpath(webMobileLocator).also {
                if ((it.isUnderneathFixedElements()).or(!it.isCurrentlyVisible)) {
                    scrollTo(it)
                }
            }
            else -> throw IllegalArgumentException("Unknown element locator strategy.")
        }
    }

    fun scrollToElement(): HybridPageElement {
        actOnTheElement { scrollTo(it) }
        return this
    }

    open fun click() {
        waitForElementToBecomeVisible().actOnTheElement {
            scrollTo(it)
            it.click()
        }
    }

    protected fun scrollTo(elem: Any) {
        val jsExecutor = page.driver as JavascriptExecutor
        try {
            jsExecutor.executeScript("arguments[0].scrollIntoView({block: \"center\"});", elem)
        } catch (e: NoSuchElementException) {
            throw NoSuchElementException("Error scrolling to $elem.  " +
                    "No such element existed on the page.  " +
                    "Page source:\n${page.driver.pageSource}\n")
        }
    }

    val elements: List<WebElementFacade>
        get() {
            var elements: List<WebElementFacade>
            var retryCount = (timeToWaitForElement / ELEMENT_RETRY_TIME).toInt()
            while(true) {
                elements = when (locatorStrategy()) {
                    LOCATOR_STRATEGY_IOS -> page.findAllByXpath(iOSLocator!!)
                    LOCATOR_STRATEGY_ANDROID -> page.findAllByXpath(androidLocator!!)
                    LOCATOR_STRATEGY_WEBVIEW,
                    LOCATOR_STRATEGY_BROWSER_DESKTOP -> page.findAllByXpath(webDesktopLocator)
                    LOCATOR_STRATEGY_BROWSER_MOBILE -> page.findAllByXpath(webMobileLocator)
                    else -> throw IllegalArgumentException("Unknown element locator strategy.")
                }
                retryCount--

                if(elements.count()>0 || retryCount<=0)
                    break

                Thread.sleep((ELEMENT_RETRY_TIME * MILLISECONDS_IN_A_SECOND).toLong())
            }
            return elements
        }

    fun waitForElement(numberOfRetries: Int = DEFAULT_RETRIES,
                               waitTime: Long = DEFAULT_WAIT_TIME): HybridPageElement {
        //These pages are throwing stale exceptions when interacting with them
        //By waiting for specific elements, we ensure that the page is fully loaded
        var retryCountdown = numberOfRetries
        var staleElement = true
        while (staleElement || retryCountdown > 0) {
            try {
                assertIsVisible()
                staleElement = false
                retryCountdown=0
            } catch (e: StaleElementReferenceException) {
                println("Could not find element. RETRYING")
                staleElement = true
            } catch (e: AssertionError) {
                println("Could not find element. RETRYING")
                Thread.sleep(waitTime)
                retryCountdown--
                if(retryCountdown==0)
                    throw e
            }
        }
        return this
    }

    fun actOnTheElement(actionOn: (elem: WebElementFacade) -> Unit) {
        var retryAssertionsOnce = 2
        while(retryAssertionsOnce>0) {
            try {
                actionOn(selectElement())
                break
            }
            catch(e: StaleElementReferenceException){
                Thread.sleep(MILLISECONDS_IN_A_SECOND)
            }
            catch(e: AssertionError) {
                retryAssertionsOnce--
                if(retryAssertionsOnce==0)
                    throw(e)
                Thread.sleep(MILLISECONDS_IN_A_SECOND)
            }
        }
    }

    fun locatorStrategy(): String {
        return if (page.driver.isAndroid() && androidLocator != null) {
            LOCATOR_STRATEGY_ANDROID
        } else if (page.driver.isIOS() && iOSLocator != null) {
            LOCATOR_STRATEGY_IOS
        } else if (!page.driver.isIOS() &&
                !page.driver.isAndroid() &&
               OptionManager.instance().isEnabled(DeviceWebMobile::class)){
               LOCATOR_STRATEGY_BROWSER_MOBILE
            }
        else {
            LOCATOR_STRATEGY_BROWSER_DESKTOP
        }
    }

    fun withText(text: String, exact: Boolean = true): HybridPageElement {

        return when (exact) {
            true -> {
                HybridPageElement(
                        webDesktopLocator = this.webDesktopLocator.plus("[text()=\"$text\"]"),
                        androidLocator = this.androidLocator?.plus("[@text=\"$text\"]"),
                        iOSLocator = this.iOSLocator?.plus("[@text=\"$text\"]"),
                        helpfulName = this.helpfulNameToUse,
                        page = this.page)
            }
            false -> {
                HybridPageElement(
                        webDesktopLocator = this.webDesktopLocator.plus("[contains(text(),\"$text\")]"),
                        androidLocator = this.androidLocator?.plus("[contains(@text,\"$text\")]"),
                        iOSLocator = this.iOSLocator?.plus("[contains(@text,\"$text\")]"),
                        helpfulName = this.helpfulNameToUse,
                        page = this.page)
            }
        }
    }

    override fun toString(): String {
        return StringBuilder(HybridPageElement::class.simpleName)
                .append(" { ")
                .append("webDesktopLocator: $webDesktopLocator, ")
                .append("webMobileLocator: $webMobileLocator, ")
                .append("androidLocator: $androidLocator, ")
                .append("iOSLocator: $iOSLocator ")
                .append("}")
                .toString()
    }

    private fun WebElementFacade.isUnderneathFixedElements(): Boolean {

        val element = this.getWrappedElementWithRetry()

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

    private fun WebElementFacade.getWrappedElementWithRetry() : WebElement {
        var retryCount = (timeToWaitForElement / ELEMENT_RETRY_TIME).toInt()

        var wrappedElement: WebElement? = null

        while(retryCount>=0) {
            try {
                wrappedElement = this.wrappedElement
                break
            } catch (exception: NoSuchElementException) {
                when(retryCount) {
                    0 -> throw exception
                    else -> {
                        retryCount--
                        println("Could not find element. RETRYING")
                        Thread.sleep((ELEMENT_RETRY_TIME * MILLISECONDS_IN_A_SECOND).toLong())
                    }
                }
            }
        }
        return wrappedElement!!
    }

    val textValue: String
        get() {
            var text = ""
            actOnTheElement { text = it.text }
            return text
        }

   fun sendEnterKey() {
       this.waitForElementToBecomeVisible().sendKeys(Keys.ENTER)
   }
}


