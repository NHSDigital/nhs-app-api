package pages

import config.Config
import net.serenitybdd.core.pages.PageObject
import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.JavascriptExecutor
import org.openqa.selenium.NoSuchElementException
import pages.sharedElements.BannerObject
import utils.PageLogging
import java.util.*

open class HybridPageObject : PageObject() {

    val containsTextXpathSubstring = "[contains(text(), \"%s\")]"

    val validationBanner by lazy { BannerObject.error(this) }

    val pageLogging by lazy { PageLogging(driver) }

    fun findAllByXpath(xpath: String): List<WebElementFacade> {
        pageLogging.logSelectorAndSource(xpath)
        return findAll(By.xpath(xpath))
    }

    fun findByXpath(xpath: String): WebElementFacade {
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
                   helpfulName: String? = null,
                   timeToWaitForElement: Int = TIME_TO_WAIT_FOR_ELEMENT): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator,
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

    override fun <T : PageObject?> switchToPage(pageObjectClass: Class<T>?): T {
        val page = super.switchToPage(pageObjectClass)
        return page
    }

    fun clickOnButtonContainingText(text: String) {
        getElement(
                webDesktopLocator = "//button",
                timeToWaitForElement = 30
        ).withText(text, false).assertIsVisible().click()
    }

    fun clickOnBackLink() {
        getBackLink().click()
    }

    fun getBackLink(): HybridPageElement {
        return getElement("//a[@data-purpose='main-back-button']")
    }

    fun clickOnExpanderLinkContainingText(text: String) {
        getElement(
            webDesktopLocator = "//div",
            timeToWaitForElement = 30
        ).withText(text, false).assertIsVisible().click()
    }

    fun WebElementFacade.getTextWithoutUnicodeSuffix(): String{
        val charValToRemove = ("\u200B")
        return this.text.removeSuffix(charValToRemove)
    }

    fun assertLinkExists(url: String, selector: String): HybridPageElement {
        val link = getElement(selector)
        link.actOnTheElement { l -> Assert.assertEquals("link", url, l.getAttribute("href")) }
        return link
    }

    fun assertLinkExists(linkTitle: String, url: String, internal: Boolean): HybridPageElement {
        val selector = "//a[contains(.,'$linkTitle')]"
        val expectedLink = if(internal) { Config.instance.url + url} else {url}
        return assertLinkExists(expectedLink, selector)
    }

    fun assertContactUsLinkExists(url: String? = null) : HybridPageElement {
        var locator = "//a[contains(text(),'Contact us')]"
        var message: String? = null

        if (!url.isNullOrBlank()) {
            locator += "[starts-with(@href, '$url')]"
            message = "Expected the link called Contact us with target of $url to be available"
        }
        return getElement(locator).assertIsVisible(message)
    }

    fun attachJavascriptFunctionsToNativeAppWindow(scripts: ArrayList<String>){
        val attachToNativeAppWindow = "window.nativeApp = {${scripts.joinToString( ",")}}"
        val jsExecutor = this.driver as JavascriptExecutor
        jsExecutor.executeScript(attachToNativeAppWindow)
    }
}
