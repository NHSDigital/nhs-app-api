package pages

import config.Config
import io.appium.java_client.AppiumDriver
import io.appium.java_client.android.AndroidDriver
import io.appium.java_client.ios.IOSDriver
import net.serenitybdd.core.pages.PageObject
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.webdriver.SerenityWebdriverManager
import net.thucydides.core.webdriver.WebDriverFacade
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.NoSuchElementException
import org.openqa.selenium.StaleElementReferenceException
import org.openqa.selenium.support.ui.FluentWait
import java.time.Duration

const val DEFAULT_SPINNER_WAIT: Long = 30
const val DEFAULT_MOBILE_WAIT: Long = 5000
const val DEFAULT_NATIVE_SPINNER_WAIT: Long = 1000
const val POOLING_FREQUENCY: Long = 100
const val WEB_CONTEXT: String = "webview"

@Suppress("TooManyFunctions")
open class HybridPageObject : PageObject() {

    val containsTextXpathSubstring = "[contains(text(), \"%s\")]"

    val errorBanner by lazy { ErrorBannerPageObject(this) }

    val spinner = HybridPageElement(
            browserLocator = "//*[@id='loading-spinner']",
            androidLocator = "//ProgressBar",
            iOSLocator = "//*[@class='nuxt-progress']",
            page = this
    )

    fun waitForSpinnerToDisappear(seconds: Long = DEFAULT_SPINNER_WAIT) {
        if(onMobile())
            spinner.waitForNativeSpinner()

        if (!spinner.elements.isEmpty()) spinner.shouldNotBeVisible(seconds)
    }

    private fun HybridPageElement.waitForNativeSpinner(
            milliseconds: Long = DEFAULT_NATIVE_SPINNER_WAIT): WebElementFacade {
        //waiting for the native spinner can cause problems
        //because it can vanish and comeback
        Thread.sleep(milliseconds)
        return this.element
    }

    fun waitForNativeStepToComplete(milliseconds: Long = DEFAULT_MOBILE_WAIT){
        //Native execution/redirect is slow on browser stack
       if(onMobile())
            Thread.sleep(milliseconds)
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

    fun onMobile(): Boolean {
        return if (SerenityWebdriverManager.inThisTestThread().hasAnInstantiatedDriver()) {
            isAndroid().xor(isIOS())

        } else { //no driver yet instantiated so check the environment variables

            val pathMatchesBrowserstack = Regex("bs://[a-z0-9]+").matches(Config.instance.appPath)
            val pathMatchesLocalApk = Regex("[A-Z]:\\\\.+\\.apk").matches(Config.instance.appPath)

            pathMatchesBrowserstack.xor(pathMatchesLocalApk)
        }
    }

    fun isAndroid(): Boolean {
        val isAndroid = driver is AndroidDriver<*>
        val isProxyForAndroid = when (driver is WebDriverFacade) {
            true -> { (driver as WebDriverFacade).isAProxyFor(AndroidDriver::class.java) }
            false -> { false }
        }
        return isAndroid.xor(isProxyForAndroid)
    }

    fun isIOS(): Boolean {
        val isIOS = driver is IOSDriver<*>
        val isProxyForIOS = when  (driver is WebDriverFacade) {
            true -> { (driver as WebDriverFacade).isAProxyFor(IOSDriver::class.java) }
            false -> { false }
        }
        return isIOS.xor(isProxyForIOS)
    }

    fun switchWebview() {
        if (!onMobile())
            return

        val driver = getMobileDriver()
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

    fun getMobileDriver(): AppiumDriver<WebElementFacade>{
        return when (isAndroid()) {
            true -> {  getSpecificDriver<AndroidDriver<WebElementFacade>>() }
            false -> { getSpecificDriver<IOSDriver<WebElementFacade>>() }
        }
    }

    fun <T>getSpecificDriver(): T {
        val theDriver =
                if (driver is WebDriverFacade) {
                    (driver as WebDriverFacade).proxiedDriver
                } else {
                    driver
                }
        @Suppress("UNCHECKED_CAST",
                "Cast cannot be checked as a generic type is used, " +
                        "see https://kotlinlang.org/docs/reference/typecasts.html")
        return theDriver as T
    }

    fun findByXpath(parent: WebElementFacade, xpath: String): WebElementFacade {
        val elements = findAllByXpath(parent, xpath)

        if(!elements.any()){
            Assert.fail("No elements found for '$xpath'")
        }
        return elements.first()
    }

    fun logSelectorAndSource(selector:String){
        if(Config.instance.showPageSourceForXPathQuery=="true") {
            println("Selector: $selector")
            println("Current source:\n${driver.pageSource}")
        }
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

    fun findAllByXpath(parent: WebElementFacade, xpath: String): List<WebElementFacade> {
        switchWebview()
        logSelectorAndSource(xpath)
        return parent.thenFindAll(xpath)
    }

    fun findAllByXpath(xpath: String): List<WebElementFacade> {
        switchWebview()
        logSelectorAndSource(xpath)
        return findAll(By.xpath(xpath))
    }

    fun clickOnButtonContainingText(text: String) {
        HybridPageElement(
                browserLocator = "//button",
                androidLocator = null,
                page = this
        )
                .withText(text, false).assertIsVisible().click()
    }
}
