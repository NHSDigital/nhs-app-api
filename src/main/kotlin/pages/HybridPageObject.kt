package pages

import io.appium.java_client.android.AndroidDriver
import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.annotations.findby.How
import net.serenitybdd.core.pages.PageObject
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.webdriver.WebDriverFacade
import org.junit.Assert
import org.openqa.selenium.By


abstract class HybridPageObject(private var pageType: PageType) : PageObject() {

    override fun <T : PageObject?> switchToPage(pageObjectClass: Class<T>?): T {
        val page = super.switchToPage(pageObjectClass)
        this.pageType = (page as HybridPageObject).pageType
        return page
    }

    fun onMobile(): Boolean {
        val webDriverFacade = driver as WebDriverFacade
        if (webDriverFacade.isAProxyFor(AndroidDriver::class.java)) {
            return true
        }
        return false
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
        val originalDriver = (driver as WebDriverFacade).proxiedDriver
        val appiumDriver = (originalDriver as AndroidDriver<WebElementFacade>)
        for (context in appiumDriver.contextHandles) {
            if (context.contains(name, true)) {
                println("Switching context... Currently on: ${appiumDriver.context}")
                // println("Original page: ${appiumDriver.pageSource}")
                appiumDriver.context(context)
                println("Switched context! Now on: ${appiumDriver.context}")
                // println("New page: ${appiumDriver.pageSource}")
                println("Current window: ${appiumDriver.windowHandle}")
                println("All windows: ${appiumDriver.windowHandles}")
                println("Switching window to default...")
                appiumDriver.switchTo().defaultContent()
                println("Current window: ${appiumDriver.windowHandle}")
                println("All windows: ${appiumDriver.windowHandles}")
            }
        }
        setDriver<HybridPageObject>(appiumDriver)
        return appiumDriver
    }

    fun findByXpath(parent: WebElementFacade, xpath: String): WebElementFacade {
        if (onMobile()) switchView()
        return parent.findBy(By.xpath(xpath))
    }

    fun findByXpath(xpath: String): WebElementFacade {
        if (onMobile()) switchView()
        return findBy(xpath)
    }

    fun findAllByXpath(parent: WebElementFacade, xpath: String):List<WebElementFacade> {
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

    @FindBy(how = How.XPATH, using = "//*[@id='loading-spinner']")
    lateinit var spinner: WebElementFacade

    fun spinnerVisible(): Boolean {
        return spinner.isCurrentlyVisible
    }

    fun getErrorText(): String {
        val paragraphs = findAllByXpath("//div[@class='msg error']//p")
        var content = StringBuilder()

        paragraphs.forEach( { el ->
            if ( el.isVisible ) {
                val t = el.text
                if (t != null) {
                    content.append(t)
                }
            }
        })

        return content.toString()
    }

    fun getRetryButtonText() : String {
        val buttons = findAllByXpath("//div[@class='msg error']//button")
        Assert.assertEquals(1, buttons.size)
        return buttons[0].text
    }
}