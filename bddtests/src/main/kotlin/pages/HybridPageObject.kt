package pages

import config.Config
import net.serenitybdd.core.pages.PageObject
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.webdriver.SerenityWebdriverManager
import org.openqa.selenium.By
import org.openqa.selenium.NoSuchElementException
import org.openqa.selenium.StaleElementReferenceException
import org.openqa.selenium.support.ui.FluentWait
import webdrivers.getMobileDriver
import webdrivers.isAndroid
import webdrivers.isIOS
import java.time.Duration
import pages.sharedElements.BannerObject

const val DEFAULT_SPINNER_WAIT: Long = 30
const val DEFAULT_MOBILE_WAIT: Long = 2000
const val DEFAULT_VISIBILITY_WAIT: Long = 300
const val DEFAULT_NATIVE_SPINNER_WAIT: Long = 1000
const val POOLING_FREQUENCY: Long = 100
const val WEB_CONTEXT: String = "webview"

open class HybridPageObject : PageObject() {

    val containsTextXpathSubstring = "[contains(text(), \"%s\")]"

    val validationBanner by lazy { BannerObject.error(this) }

    val spinner = HybridPageElement(
            webDesktopLocator = "//*[@id='loading-spinner']",
            webMobileLocator = "//*[@id='loading-spinner']",
            androidLocator = "//ProgressBar",
            iOSLocator = "//*[@class='nuxt-progress']",
            page = this
    )

    fun waitForSpinnerToDisappear(seconds: Long = DEFAULT_SPINNER_WAIT) {
        if (onMobile()) {
            spinner.waitForNativeSpinner()
        }

        if (!spinner.elements.isEmpty() && !onMobile()) {
            spinner.shouldNotBeVisible(seconds)
        }
    }

    fun waitForNativeStepToComplete(milliseconds: Long = DEFAULT_MOBILE_WAIT) {
        //Native execution/redirect is slow on browser stack
        if (onMobile()) {
            Thread.sleep(milliseconds)
        }
    }

    private fun HybridPageElement.shouldNotBeVisible(seconds: Long = DEFAULT_SPINNER_WAIT) {
        try {
            val currentElement = this.element
            FluentWait<WebElementFacade>(currentElement)
                    .withTimeout(Duration.ofSeconds(seconds))
                    .pollingEvery(Duration.ofMillis(POOLING_FREQUENCY))
                    .until {
                        !it.isPresent || !it.isCurrentlyVisible
                    }
        } catch (e: NoSuchElementException) {
            // continue
        } catch (e: StaleElementReferenceException) {
            // continue
        }
    }

    fun findAllByXpath(xpath: String): List<WebElementFacade> {
        switchWebview()
        logSelectorAndSource(xpath)
        return findAll(By.xpath(xpath))
    }


    fun findByXpath(xpath: String): WebElementFacade {
        switchWebview()
        val element: WebElementFacade
        try {
            logSelectorAndSource(xpath)
            element = findBy(xpath)
        } catch (e: NoSuchElementException) {
            throw NoSuchElementException("No element found on page:\n${driver.pageSource}", e)
        }
        return element
    }

    fun switchWebview() {
        if (!onMobile())
            return

        val driver = driver.getMobileDriver()
        if (driver.context.contains(WEB_CONTEXT, ignoreCase = true)) {
            println("Already in ${WEB_CONTEXT} context: ${driver.context}")
        } else {
            for (context in driver.contextHandles) {
                if (context.contains(WEB_CONTEXT, true)) {
                    println("Switching context to $context... Currently on: ${driver.context}")
                    driver.context(context)
                    println("Switched context! Now on: ${driver.context}")
                    break
                }
            }
        }
        setDriver<HybridPageObject>(driver)
    }

    fun logSelectorAndSource(selector: String) {
        if (Config.instance.showPageSourceForXPathQuery == "true") {
            println("Selector: $selector")
            println("Current source:\n${driver.pageSource}")
        }
    }

    fun shouldBeVisibleOnNative(elementToCheckFor:HybridPageElement, seconds: Long = DEFAULT_VISIBILITY_WAIT){
        try {
            waitForSpinnerToDisappear()
            val currentElement = elementToCheckFor.element
            FluentWait<WebElementFacade>(currentElement)
                    .withTimeout(Duration.ofSeconds(seconds))
                    .pollingEvery(Duration.ofMillis(POOLING_FREQUENCY))
                    .until {
                        currentElement.isPresent
                    }
            if (!currentElement.isVisible) {
                waitForNativeStepToComplete()
            }
        } catch (e: NoSuchElementException) {
            throw NoSuchElementException("Element $elementToCheckFor does not exist on the page.  " +
                    "Page source:\n${driver.pageSource}\n")
        }
    }

    override fun <T : PageObject?> switchToPage(pageObjectClass: Class<T>?): T {
        val page = super.switchToPage(pageObjectClass)
        return page
    }

    fun onMobile(): Boolean {
        return if (SerenityWebdriverManager.inThisTestThread().hasAnInstantiatedDriver()) {
            driver.isAndroid().xor(driver.isIOS())

        } else { //no driver yet instantiated so check the environment variables

            val pathMatchesBrowserstack = Regex("bs://[a-z0-9]+").matches(Config.instance.appPath)
            val pathMatchesLocalApk = Regex("[A-Z]:\\\\.+\\.apk").matches(Config.instance.appPath)

            pathMatchesBrowserstack.xor(pathMatchesLocalApk)
        }
    }

    fun hideKeyboardIfOnMobile(){
        if (onMobile()) {
            driver.getMobileDriver().hideKeyboard()
        }
    }

    fun clickOnButtonContainingText(text: String) {
        HybridPageElement(
                webDesktopLocator = "//button",
                androidLocator = null,
                page = this
        )
                .withText(text, false).assertIsVisible().click()
    }

    fun clickOnLinkContainingText(text: String) {
        HybridPageElement(
                webDesktopLocator = "//a",
                androidLocator = null,
                page = this
        )
                .withText(text, false).assertIsVisible().click()
    }

    fun isButtonVisible(button: String): Boolean {
        return findBy<WebElementFacade>(
                "//button[contains(text()," +
                        "'$button')]").waitUntilVisible<WebElementFacade>().isCurrentlyVisible
    }
}

