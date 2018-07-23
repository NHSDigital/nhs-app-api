package pages

import config.Config
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
import pages.navigation.Header
import java.lang.AssertionError
import java.time.Duration

const val DEFAULT_SPINNER_WAIT: Long = 5

abstract class HybridPageObject(private var pageType: PageType) : PageObject() {

    val spinner = HybridPageElement(
            browserLocator = "//*[@id='loading-spinner']",
            androidLocator = "//ProgressBar",
            page = this
    )

    private val warningMessage = HybridPageElement(
            browserLocator = "//div[@class='msg warning']",
            androidLocator = null,
            page = this
    )

    fun waitForSpinnerToDisappear(seconds: Long = DEFAULT_SPINNER_WAIT) {
        spinner.shouldNotBeVisible(seconds)
    }

    fun HybridPageElement.waitForSpinner(seconds: Long = DEFAULT_SPINNER_WAIT): WebElementFacade {
        spinner.shouldNotBeVisible(seconds)
        return this.element
    }

    fun HybridPageElement.shouldNotBeVisible(seconds: Long = DEFAULT_SPINNER_WAIT) {
        try {
            FluentWait<WebElementFacade>(this.element)
                    .withTimeout(Duration.ofSeconds(seconds))
                    .pollingEvery(Duration.ofMillis(100))
                    .until {
                        !it.isCurrentlyVisible
                    }
        } catch (e: NoSuchElementException) {
            // continue
        } catch (e: StaleElementReferenceException) {
            // continue
        }
    }

    override fun <T : PageObject?> switchToPage(pageObjectClass: Class<T>?): T {
        val page = super.switchToPage(pageObjectClass)
        this.pageType = (page as HybridPageObject).pageType
        return page
    }

    fun onMobile(): Boolean {
        if (SerenityWebdriverManager.inThisTestThread().hasAnInstantiatedDriver()) {
            return isAndroid().xor(isIOS())

        } else { //no driver yet instantiated so check the environment variables

            val pathMatchesBrowserstack = Regex("bs://[a-z0-9]+").matches(Config.instance.appPath)
            val pathMatchesLocalApk = Regex("[A-Z]:\\\\.+\\.apk").matches(Config.instance.appPath)

            return pathMatchesBrowserstack.xor(pathMatchesLocalApk)
        }
    }

    fun isAndroid(): Boolean {
        val isAndroid = driver is AndroidDriver<*>
        val isProxyForAndroid: Boolean
        if (driver is WebDriverFacade) {
            isProxyForAndroid = (driver as WebDriverFacade).isAProxyFor(AndroidDriver::class.java)
        } else {
            isProxyForAndroid = false
        }

        return isAndroid.xor(isProxyForAndroid)
    }

    fun isIOS(): Boolean {
        val isIOS = driver is IOSDriver<*>
        val isProxyForIOS: Boolean
        if (driver is WebDriverFacade) {
            isProxyForIOS = (driver as WebDriverFacade).isAProxyFor(IOSDriver::class.java)
        } else {
            isProxyForIOS = false
        }

        return isIOS.xor(isProxyForIOS)
    }

    private fun switchView(): AndroidDriver<WebElementFacade> {
        return when (this.pageType) {
            PageType.WEBVIEW_APP -> {
                switchContext("nhsonline")
            }
            PageType.WEBVIEW_BROWSER -> {
                switchContext("chrome")
            }
            PageType.NATIVE -> {
                switchContext("native")
            }
        }
    }

    private fun switchContext(name: String): AndroidDriver<WebElementFacade> {
        val originalDriver = getAndroidDriver()

        if (originalDriver.context.contains(name, ignoreCase = true)) {
            println("Already in $name context: ${originalDriver.context}")
        } else {
            for (context in originalDriver.contextHandles) {
                if (context.contains(name, true)) {
                    println("Switching context to $context... Currently on: ${originalDriver.context}")
                    originalDriver.context(context)
                    println("Switched context! Now on: ${originalDriver.context}")

                    if (name.contains("chrome")) {
                        switchToDefaultWindow(originalDriver)
                    }
                }
            }
        }
        setDriver<HybridPageObject>(originalDriver)
        return originalDriver
    }

    private fun getAndroidDriver(): AndroidDriver<WebElementFacade> {
        if (driver is WebDriverFacade) {
            return ((driver as WebDriverFacade).proxiedDriver) as AndroidDriver<WebElementFacade>
        } else {
            return driver as AndroidDriver<WebElementFacade>
        }
    }

    private fun switchToDefaultWindow(originalDriver: AndroidDriver<WebElementFacade>) {
        println("Current window: ${originalDriver.windowHandle}")
        println("All windows: ${originalDriver.windowHandles}")
        println("Switching window to default...")
        originalDriver.switchTo().defaultContent()
        println("Current window: ${originalDriver.windowHandle}")
        println("All windows: ${originalDriver.windowHandles}")
    }

    fun findByXpath(parent: WebElementFacade, xpath: String): WebElementFacade {
        if (onMobile()) switchView()
        val element: WebElementFacade
        try {
            element = parent.findBy(By.xpath(xpath))
        } catch (e: NoSuchElementException) {
            throw NoSuchElementException("No element found on page:\n${driver.pageSource}", e)
        }
        return element
    }

    fun findByXpath(xpath: String): WebElementFacade {
        if (onMobile()) switchView()
        val element: WebElementFacade
        try {
            element = findBy(xpath)
        } catch (e: NoSuchElementException) {
            throw NoSuchElementException("No element found on page:\n${driver.pageSource}", e)
        }
        return element
    }

    fun findAllByXpath(parent: WebElementFacade, xpath: String): List<WebElementFacade> {
        if (onMobile()) switchView()
        return parent.thenFindAll(xpath)
    }

    fun findAllByXpath(xpath: String): List<WebElementFacade> {
        if (onMobile()) switchView()
        return findAll(By.xpath(xpath))
    }

    companion object {
        enum class PageType {
            WEBVIEW_APP,
            WEBVIEW_BROWSER,
            NATIVE
        }
    }

    fun getErrorDetailText(): String? {
        return try {
            switchToPage(ErrorPage::class.java)
                    .detailTwo.element.text
        } catch (e: NoSuchElementException) {
            null
        }
    }

    fun getWarningText(): String? {
        return try {
            warningMessage.element.text
        } catch (e: NoSuchElementException) {
            null
        }
    }

    fun getRetryButtonText(): String {
        val buttons = findAllByXpath("//div[@class='msg error']//button")
        Assert.assertEquals(1, buttons.size)
        return buttons[0].text
    }

    fun isErrorMessageContentCorrect(pageTitle: String,
                                     pageHeaderText: String,
                                     headerText: String,
                                     subHeaderText: String,
                                     messageText: String,
                                     retryButtonText: String): Boolean {

        var pageHeadIsValid = true
        var headerIsValid = true
        var subHeaderIsValid = true
        var messageIsValid = true
        var retryButtonIsValid = true

        if (!pageHeaderText.isNullOrEmpty()) {
            pageHeadIsValid = isAnyXpathVisible("//h1[contains(text(), \"$pageHeaderText\")]")
        }
        if (!headerText.isNullOrEmpty()) {
            headerIsValid = isAnyXpathVisible("//p[contains(text(), \"$headerText\")]")
        }
        if (!subHeaderText.isNullOrEmpty()) {
            subHeaderIsValid = isAnyXpathVisible("//p[contains(text(), \"$subHeaderText\")]")
        }
        if (!messageText.isNullOrEmpty()) {
            messageIsValid = isAnyXpathVisible("//p[contains(text(), \"$messageText\")]")
        }
        if (!retryButtonText.isNullOrEmpty()) {
            retryButtonIsValid = isAnyXpathVisible("//button[contains(text(), \"$retryButtonText\")]")
        }

        return pageHeadIsValid && headerIsValid && subHeaderIsValid && messageIsValid && retryButtonIsValid
    }

    fun waitForPageHeaderText(expectedHeaderText: String): Boolean {
        return waitFor({ getPageHeaderText() == expectedHeaderText }) != null
    }

    fun getPageHeaderText(): String {
        return switchToPage(Header::class.java).pageTitle.element.text
    }

    fun clickOnButtonContainingText(text: String) {
       HybridPageElement(
            browserLocator = "//div[@id='app']//button",
            androidLocator = null,
            page = this
       )
       .containingText(text)
       .element
       .click()
    }

    private fun isAnyXpathVisible(xpath: String): Boolean {

        val allElements = findAllByXpath(xpath)

        var anyVisible = false

        allElements.forEach {
            if (it.isVisible) {
                anyVisible = true
            }
        }


        return anyVisible
    }

    fun hideKeyboard() {
        if (isAndroid()) {
            getAndroidDriver().hideKeyboard()
        } else if (isIOS()) {
            throw NotImplementedError("IOS keyboard hiding not yet implemented.")
        }
    }
}