package pages

import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.JavascriptExecutor
import org.openqa.selenium.Keys
import org.openqa.selenium.NoSuchElementException
import org.openqa.selenium.StaleElementReferenceException
import org.openqa.selenium.WebElement
import webdrivers.getLocatorStrategy

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
        val helpfulName: String? = null,
        var timeToWaitForElement: Int = TIME_TO_WAIT_FOR_ELEMENT
) {
    var helpfulNameToUse = helpfulName ?: webDesktopLocator

    val textValue: String
        get() {
            var text = ""
            actOnTheElement { text = it.text }
            return text
        }

    val elements: List<WebElementFacade>
        get() {
            return getElements { element -> element }
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

    fun <T> getElements(converter: (WebElementFacade) -> T): List<T> {
        var retryCount = numberOfRetries()
        var staleElement = false
        while (staleElement || retryCount > 0) {
            try {
                staleElement = false
                val foundElements = getElementsWithLocatorMethod()
                if (foundElements.count() > 0) {
                    return foundElements.map { element -> converter.invoke(element) }
                }
                retryCount--
                Thread.sleep((ELEMENT_RETRY_TIME * MILLISECONDS_IN_A_SECOND).toLong())
            } catch (e: NoSuchElementException) {
                retryCount--
                println("Could not find element. Retry Count: $retryCount")
                Thread.sleep((ELEMENT_RETRY_TIME * MILLISECONDS_IN_A_SECOND).toLong())
            } catch (e: StaleElementReferenceException) {
                staleElement = true
                println("Could not find element. Retry Count: $retryCount")
            }
        }
        return listOf()
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
                retryCountdown = 0
            } catch (e: StaleElementReferenceException) {
                println("Could not find element. Retry Count: $retryCountdown")
                staleElement = true
            } catch (e: AssertionError) {
                println("Could not find element. Retry Count: $retryCountdown")
                Thread.sleep(waitTime)
                retryCountdown--
                if (retryCountdown == 0)
                    throw e
            }
        }
        return this
    }

    fun actOnTheElement(actionOn: (elem: WebElementFacade) -> Unit) {
        var retryAssertionsOnce = 2
        while (retryAssertionsOnce > 0) {
            try {
                actionOn(selectElement())
                break
            } catch (e: StaleElementReferenceException) {
                Thread.sleep(MILLISECONDS_IN_A_SECOND)
            } catch (e: AssertionError) {
                retryAssertionsOnce--
                if (retryAssertionsOnce == 0)
                    throw(e)
                Thread.sleep(MILLISECONDS_IN_A_SECOND)
            }
        }
    }

    fun withText(text: String, exact: Boolean = true, normalised: Boolean = false): HybridPageElement {
        val textFunc = if (normalised) "normalize-space(text())" else "text()"
        val textAttribute = if (normalised) "normalize-space(@text)" else "@text"
        return when (exact) {
            true -> {
                HybridPageElement(
                        webDesktopLocator = this.webDesktopLocator.plus("[$textFunc=\"$text\"]"),
                        webMobileLocator = this.webMobileLocator.plus("[$textFunc=\"$text\"]"),
                        androidLocator = this.androidLocator?.plus("[$textAttribute=\"$text\"]"),
                        iOSLocator = this.iOSLocator?.plus("[$textAttribute=\"$text\"]"),
                        helpfulName = this.helpfulNameToUse,
                        page = this.page,
                        timeToWaitForElement = this.timeToWaitForElement)
            }
            false -> {
                HybridPageElement(
                        webDesktopLocator = this.webDesktopLocator.plus("[contains($textFunc,\"$text\")]"),
                        webMobileLocator = this.webMobileLocator.plus("[$textFunc=\"$text\"]"),
                        androidLocator = this.androidLocator?.plus("[contains($textAttribute,\"$text\")]"),
                        iOSLocator = this.iOSLocator?.plus("[contains($textAttribute,\"$text\")]"),
                        helpfulName = this.helpfulNameToUse,
                        page = this.page,
                        timeToWaitForElement = this.timeToWaitForElement)
            }
        }
    }

    fun sendEnterKey() {
        this.waitForElementToBecomeVisible().sendKeys(Keys.ENTER)
    }

    protected fun setHelpfulNameToUseFromLocator(locator: String) {
        if (helpfulName != null) {
            helpfulNameToUse = "$helpfulName ($locator)"
        } else {
            helpfulNameToUse = locator
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

    private fun WebElementFacade.getWrappedElementWithRetry(): WebElement {
        var retryCount = numberOfRetries()
        var wrappedElement: WebElement? = null

        while (retryCount >= 0) {
            try {
                wrappedElement = this.wrappedElement
                break
            } catch (exception: NoSuchElementException) {
                when (retryCount) {
                    0 -> throw exception
                    else -> {
                        retryCount--
                        println("Could not find element. Retry Count: $retryCount")
                        Thread.sleep((ELEMENT_RETRY_TIME * MILLISECONDS_IN_A_SECOND).toLong())
                    }
                }
            }
        }
        return wrappedElement!!
    }

    private fun numberOfRetries() =
            if (timeToWaitForElement == 0) 1 else (timeToWaitForElement / ELEMENT_RETRY_TIME).toInt()

    private fun selectElement(): WebElementFacade {
        return when (page.driver.getLocatorStrategy(this)) {
            LocatorStrategy.IOS -> page.findByXpath(iOSLocator!!).also {
                it.getWrappedElementWithRetry()
            }
            LocatorStrategy.ANDROID -> page.findByXpath(androidLocator!!).also {
                it.getWrappedElementWithRetry()
            }
            LocatorStrategy.WEBVIEW,
            LocatorStrategy.BROWSER_DESKTOP -> page.findByXpath(webDesktopLocator).also {
                if ((it.isUnderneathFixedElements()).or(!it.isCurrentlyVisible)) {
                    scrollTo(it)
                }
            }
            LocatorStrategy.BROWSER_MOBILE -> page.findByXpath(webMobileLocator).also {
                if ((it.isUnderneathFixedElements()).or(!it.isCurrentlyVisible)) {
                    scrollTo(it)
                }
            }
            else -> throw IllegalArgumentException("Unknown element locator strategy.")
        }
    }

    private fun getElementsWithLocatorMethod(): List<WebElementFacade> {
        return when (page.driver.getLocatorStrategy(this)) {
            LocatorStrategy.IOS -> page.findAllByXpath(iOSLocator!!)
            LocatorStrategy.ANDROID -> page.findAllByXpath(androidLocator!!)
            LocatorStrategy.WEBVIEW,
            LocatorStrategy.BROWSER_DESKTOP -> page.findAllByXpath(webDesktopLocator)
            LocatorStrategy.BROWSER_MOBILE -> page.findAllByXpath(webMobileLocator)
            else -> throw IllegalArgumentException("Unknown element locator strategy.")
        }
    }
}