package pages

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.webdriver.UnsupportedDriverException
import org.openqa.selenium.JavascriptExecutor
import org.openqa.selenium.NoSuchElementException

private const val LOCATOR_STRATEGY_ANDROID = "ANDROID"
private const val LOCATOR_STRATEGY_WEBVIEW = "WEBVIEW"
private const val LOCATOR_STRATEGY_BROWSER = "BROWSER"

private const val HEADER_HEIGHT_PX = 100
private const val FLOATING_BUTTON_HEIGHT_PX = 78.5
private const val NAVBAR_HEIGHT_PX = 70

class HybridPageElement (
        private var browserLocator: String,
        private var androidLocator: String?,
        private val page: HybridPageObject
){
    val element: WebElementFacade get() {
        val element: WebElementFacade

        element = when(locatorStrategy()) {
            LOCATOR_STRATEGY_ANDROID -> page.findByXpath(androidLocator!!)
            LOCATOR_STRATEGY_WEBVIEW,
            LOCATOR_STRATEGY_BROWSER -> page.findByXpath(browserLocator).also {
                if ((!it.isCurrentlyVisible).or(it.isUnderneathFixedElements())) {
                    val jsExecutor = page.driver as JavascriptExecutor
                    try {
                        jsExecutor.executeScript("arguments[0].scrollIntoView({block: \"center\"});", it)
                    } catch (e: NoSuchElementException) {
                        throw NoSuchElementException("Error scrolling to $it.  No such element existed on the page.  Page source:\n${page.driver.pageSource}\n")
                    }
                }
            }
            else -> throw IllegalArgumentException("Unknown element locator strategy.")
        }

        return element
    }

    private fun locatorStrategy(): String {
        val strategy: String

        if (page.isAndroid()) {
            if (androidLocator != null) {
                strategy = LOCATOR_STRATEGY_ANDROID
            } else {
                strategy = LOCATOR_STRATEGY_WEBVIEW
            }
        } else if (page.isIOS()) {
                throw UnsupportedDriverException("iOS Driver not yet supported")
        } else {
            strategy = LOCATOR_STRATEGY_BROWSER
        }

        return strategy
    }

    val elements: List<WebElementFacade> get() {
        if (page.onMobile()) {
            return page.findAllByXpath(androidLocator!!)
        } else {
            return page.findAllByXpath(browserLocator)
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

        val isBehindHeader = highestPixel < HEADER_HEIGHT_PX
        val isBehindFooter = lowestPixel > page.driver.manage().window().size.height -(FLOATING_BUTTON_HEIGHT_PX + NAVBAR_HEIGHT_PX)

        return isBehindHeader.or(isBehindFooter)
    }
}