package pages

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.webdriver.UnsupportedDriverException
import org.junit.Assert
import org.openqa.selenium.JavascriptExecutor
import org.openqa.selenium.NoSuchElementException
import kotlin.reflect.jvm.internal.impl.load.kotlin.JvmType

const val LOCATOR_STRATEGY_ANDROID = "ANDROID"
const val LOCATOR_STRATEGY_WEBVIEW = "WEBVIEW"
const val LOCATOR_STRATEGY_BROWSER = "BROWSER"
const val LOCATOR_STRATEGY_IOS = "IOS"
const val LOCATOR_STRATEGY_IOS_ACCESSIBILITY = "IOSACCESS"

const val HEADER_HEIGHT_PX = 100
const val FLOATING_BUTTON_HEIGHT_PX = 78.5
const val NAVBAR_HEIGHT_PX = 70

open class HybridPageElement(
        var browserLocator: String,
        var androidLocator: String? = null,
        var iOSLocator: String? = null,
        open val page: HybridPageObject,
        helpfulName: String? = null
) {
    private val helpfulNameToUse = helpfulName ?: browserLocator

    val element: WebElementFacade
        get() {
            return when (locatorStrategy()) {
                LOCATOR_STRATEGY_IOS -> page.findByXpath(iOSLocator!!)
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

    fun scrollToElement() : HybridPageElement{
        element.scroll()
        return this
    }

    fun WebElementFacade.scroll() {
        scrollTo(this)
    }

    fun scrollTo(elem: Any)
    {
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
                LOCATOR_STRATEGY_BROWSER -> page.findAllByXpath(browserLocator)
                else -> throw IllegalArgumentException("Unknown element locator strategy.")
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

    fun locatorStrategy(): String {
        return if (page.isAndroid() && androidLocator != null) {
                LOCATOR_STRATEGY_ANDROID
        } else if (page.isIOS() && iOSLocator != null) {
                LOCATOR_STRATEGY_IOS
        } else {
            LOCATOR_STRATEGY_BROWSER
        }
    }

    fun withText(text:String, exact: Boolean = true): HybridPageElement {

        return when(exact) {
            true -> {
                HybridPageElement(
                        browserLocator = this.browserLocator.plus("[text()='$text']"),
                        androidLocator = this.androidLocator?.plus("[@text='$text']"),
                        iOSLocator = this.iOSLocator?.plus("[@text='$text']"),
                        helpfulName = this.helpfulNameToUse,
                        page = this.page)
            }
            false -> {
                HybridPageElement(
                        browserLocator = this.browserLocator.plus("[contains(text(),'$text')]"),
                        androidLocator = this.androidLocator?.plus("[contains(@text,'$text')]"),
                        iOSLocator = this.iOSLocator?.plus("[contains(@text,'$text')]"),
                        helpfulName = this.helpfulNameToUse,
                        page = this.page)
            }
        }
    }

    override fun toString(): String {
        return StringBuilder(HybridPageElement::class.simpleName)
                .append(" { ")
                .append("browserLocator: $browserLocator, ")
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
}