package pages

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.webdriver.UnsupportedDriverException
import org.junit.Assert
import org.openqa.selenium.JavascriptExecutor
import org.openqa.selenium.NoSuchElementException

private const val LOCATOR_STRATEGY_ANDROID = "ANDROID"
private const val LOCATOR_STRATEGY_WEBVIEW = "WEBVIEW"
private const val LOCATOR_STRATEGY_BROWSER = "BROWSER"

private const val HEADER_HEIGHT_PX = 100
private const val FLOATING_BUTTON_HEIGHT_PX = 78.5
private const val NAVBAR_HEIGHT_PX = 70

class HybridPageElement(
        private var browserLocator: String,
        private var androidLocator: String?,
        private val page: HybridPageObject,
        helpfulName: String? = null
) {
    private val helpfulNameToUse = helpfulName ?: browserLocator

    val element: WebElementFacade
        get() {
            return when (locatorStrategy()) {
                LOCATOR_STRATEGY_ANDROID -> page.findByXpath(androidLocator!!)
                LOCATOR_STRATEGY_WEBVIEW,
                LOCATOR_STRATEGY_BROWSER -> page.findByXpath(browserLocator).also {
                    if ((!it.isCurrentlyVisible).or(it.isUnderneathFixedElements())) {
                        it.scroll()
                    }
                }
                else -> throw IllegalArgumentException("Unknown element locator strategy.")
            }
        }

    private fun WebElementFacade.scroll() {
        val jsExecutor = page.driver as JavascriptExecutor
        try {
            jsExecutor.executeScript("arguments[0].scrollIntoView({block: \"center\"});", this)
        } catch (e: NoSuchElementException) {
            throw NoSuchElementException("Error scrolling to $this.  " +
                    "No such element existed on the page.  " +
                    "Page source:\n${page.driver.pageSource}\n")
        }
    }

    val elements: List<WebElementFacade>
        get() {
            return if (page.onMobile()) {
                page.findAllByXpath(androidLocator!!)
            } else {
                page.findAllByXpath(browserLocator)
            }
        }

    fun assertSingleElementPresent(): HybridPageElement {
        Assert.assertEquals(
                "Expected only one matching element for $helpfulNameToUse, with xpath $browserLocator",
                1,
                elements.count())
        return this
    }

    fun assertIsVisible(): HybridPageElement {
        Assert.assertTrue("Expected $helpfulNameToUse to be visible", element.isVisible)
        return this
    }

    fun assertIsNotVisible(): HybridPageElement {
        Assert.assertFalse("Expected $helpfulNameToUse to not be visible", element.isVisible)
        return this
    }

    fun assertElementNotPresent(): HybridPageElement {
        Assert.assertEquals("Expected no matching elements for $helpfulNameToUse", 0, elements.count())
        return this
    }

    private fun locatorStrategy(): String {
        return if (page.isAndroid()) {
            if (androidLocator != null) {
                LOCATOR_STRATEGY_ANDROID
            } else {
                LOCATOR_STRATEGY_WEBVIEW
            }
        } else if (page.isIOS()) {
            throw UnsupportedDriverException("iOS Driver not yet supported")
        } else {
            LOCATOR_STRATEGY_BROWSER
        }
    }

    fun withText(text: String): HybridPageElement {
        this.browserLocator = this.browserLocator.plus("[text()='$text']")
        this.androidLocator = this.androidLocator?.plus("[@text='$text']")

        return this
    }

    fun containingText(text: String): HybridPageElement {
        this.browserLocator = this.browserLocator.plus("[contains(text(),'$text')]")
        this.androidLocator = this.androidLocator?.plus("[contains(@text,'$text')]")

        return this
    }

    override fun toString(): String {
        return StringBuilder(HybridPageElement::class.simpleName)
                .append(" { ")
                .append("browserLocator: $browserLocator, ")
                .append("androidLocator: $androidLocator ")
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
}