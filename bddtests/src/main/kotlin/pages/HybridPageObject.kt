package pages

import config.Config
import net.serenitybdd.core.pages.PageObject
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.webdriver.SerenityWebdriverManager
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.JavascriptExecutor
import org.openqa.selenium.NoSuchElementException
import org.openqa.selenium.WebDriverException
import pages.sharedElements.BannerObject
import utils.PageLogging
import webdrivers.getMobileDriver
import webdrivers.isAndroid
import webdrivers.isIOS
import java.util.*

const val DEFAULT_MOBILE_WAIT: Long = 5000
const val WEB_CONTEXT: String = "webview"

open class HybridPageObject : PageObject() {

    val containsTextXpathSubstring = "[contains(text(), \"%s\")]"

    val validationBanner by lazy { BannerObject.error(this) }

    val locatorMethods by lazy { LocatorMethods(this) }

    val pageLogging by lazy { PageLogging(driver) }

    val spinner = getElement(
            webDesktopLocator = "//*[@id='loading-spinner']",
            androidLocator = "//ProgressBar",
            iOSLocator = "//*[@class='nuxt-progress']"
    )

    fun findAllByXpath(xpath: String): List<WebElementFacade> {
        switchWebview()
        pageLogging.logSelectorAndSource(xpath)
        return findAll(By.xpath(xpath))
    }

    fun findByXpath(xpath: String): WebElementFacade {
        switchWebview()
        val element: WebElementFacade
        try {
            pageLogging.logSelectorAndSource(xpath)
            element = findBy(xpath)
        } catch (e: NoSuchElementException) {
            throw NoSuchElementException("No element found on page:\n${driver.pageSource}", e)
        }
        return element
    }

    fun getElement(webDesktopLocator: String,
                   androidLocator: String? = null,
                   iOSLocator: String? = null,
                   helpfulName: String? = null,
                   timeToWaitForElement: Int = TIME_TO_WAIT_FOR_ELEMENT): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator,
                androidLocator = androidLocator,
                iOSLocator = iOSLocator,
                page = this,
                helpfulName = helpfulName,
                timeToWaitForElement = timeToWaitForElement)
    }

    open fun assertPageHeader(headerText: String): HybridPageObject {
        getElement("//div[@id='content-header']//h1", helpfulName = "Page header")
                .withNormalisedText(headerText)
                .assertSingleElementPresent()
        return this
    }

    fun switchWebview() {
        if (!onMobile())
            return

        val driver = driver.getMobileDriver()
        if (driver.context.contains(WEB_CONTEXT, ignoreCase = true)) {
            println("Already in $WEB_CONTEXT context: ${driver.context}")
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
            try {
                driver.getMobileDriver().hideKeyboard()
            } catch (e: WebDriverException) {
                println("An exception was thrown hiding the device keyboard. This is not critical.")
            }
        }
    }

    fun clickOnButtonContainingText(text: String) {
        getElement(
                webDesktopLocator = "//button",
                timeToWaitForElement = 30
        ).withText(text, false).assertIsVisible().click()
    }

    fun clickOnBackLink() {
        getElement("//a[@data-purpose='main-back-button']").click()
    }

    fun WebElementFacade.getTextWithoutUnicodeSuffix(): String{
        val charValToRemove = ("\u200B")
        return this.text.removeSuffix(charValToRemove)
    }

    fun assertLinkExists(linkTitle: String, url: String, internal: Boolean): HybridPageElement {
        val link = getElement("//a[contains(.,'$linkTitle')]")
        val expectedLink = if(internal) { Config.instance.url + url} else {url}
        link.actOnTheElement { l -> Assert.assertEquals("link", expectedLink, l.getAttribute("href")) }
        return link
    }

    fun attachJavascriptFunctionsToNativeAppWindow(scripts: ArrayList<String>){
        val attachToNativeAppWindow = "window.nativeApp = {${scripts.joinToString( ",")}}"
        val jsExecutor = this.driver as JavascriptExecutor
        jsExecutor.executeScript(attachToNativeAppWindow)
    }
}
