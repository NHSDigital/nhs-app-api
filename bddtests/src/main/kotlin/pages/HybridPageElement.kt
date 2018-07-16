package pages

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.webdriver.UnsupportedDriverException
import org.openqa.selenium.JavascriptExecutor
import org.openqa.selenium.support.ui.WebDriverWait
import java.time.Duration

private const val LOCATOR_STRATEGY_ANDROID = "ANDROID"
private const val LOCATOR_STRATEGY_WEBVIEW = "WEBVIEW"
private const val LOCATOR_STRATEGY_BROWSER = "BROWSER"

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
                    val jsExecutor = page.driver as JavascriptExecutor
                    jsExecutor.executeScript("arguments[0].scrollIntoView(true);", it)
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
}