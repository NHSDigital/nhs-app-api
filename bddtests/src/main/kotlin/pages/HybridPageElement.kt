package pages

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.JavascriptExecutor
import org.openqa.selenium.Keys
import org.openqa.selenium.NoSuchElementException
import org.openqa.selenium.StaleElementReferenceException
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

open class HybridPageElement(
        var webDesktopLocator: String,
        var webMobileLocator: String = webDesktopLocator,
        var androidLocator: String? = null,
        var iOSLocator: String? = null,
        open val page: HybridPageObject,
        helpfulName: String? = null
) {
    val helpfulNameToUse = helpfulName ?: webDesktopLocator
    val element: WebElementFacade
        get() {
            return when (locatorStrategy()) {
                LOCATOR_STRATEGY_IOS -> page.findByXpath(iOSLocator!!)
                LOCATOR_STRATEGY_ANDROID -> page.findByXpath(androidLocator!!)
                LOCATOR_STRATEGY_WEBVIEW,
                LOCATOR_STRATEGY_BROWSER_DESKTOP -> page.findByXpath(webDesktopLocator).also {
                    if ((!it.isCurrentlyVisible).or(it.isUnderneathFixedElements())) {
                        it.scroll()
                    }
                }
                LOCATOR_STRATEGY_BROWSER_MOBILE -> page.findByXpath(webMobileLocator).also {
                    if ((!it.isCurrentlyVisible).or(it.isUnderneathFixedElements())) {
                        it.scroll()
                    }
                }
                else -> throw IllegalArgumentException("Unknown element locator strategy.")
            }
        }

    fun scrollToElement(): HybridPageElement {
        element.scroll()
        return this
    }

    open fun click() {
        this.element.scroll()
        this.element.click()
    }

    private fun WebElementFacade.scroll() {
        scrollTo(this)
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
            return when (locatorStrategy()) {
                LOCATOR_STRATEGY_IOS -> page.findAllByXpath(iOSLocator!!)
                LOCATOR_STRATEGY_ANDROID -> page.findAllByXpath(androidLocator!!)
                LOCATOR_STRATEGY_WEBVIEW,
                LOCATOR_STRATEGY_BROWSER_DESKTOP -> page.findAllByXpath(webDesktopLocator)
                LOCATOR_STRATEGY_BROWSER_MOBILE -> page.findAllByXpath(webMobileLocator)
                else -> throw IllegalArgumentException("Unknown element locator strategy.")
            }
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
                staleElement = true
            } catch (e: AssertionError) {
                Thread.sleep(waitTime)
                retryCountdown--
                if(retryCountdown==0)
                    throw e
            }
        }
        return this
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

        val element = this.wrappedElement

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

    fun typeTextIntoTextArea(text: String): String {
        //Each letter sent individually
        //This doesn't add a lot of time onto the test, but does help to ensure the full text is typed
        //Keys can sometimes go missing; so we return the actual text that got typed and assert that something went in
        text.toCharArray().map { letter ->
            this.element.sendKeys(letter.toString())
        }

        Assert.assertTrue("Expected some text to be output to the text area", this.element.value.isNotEmpty())

        return this.element.value
    }

   fun sendEnterKey() {
       this.element.sendKeys(Keys.ENTER)
   }
}