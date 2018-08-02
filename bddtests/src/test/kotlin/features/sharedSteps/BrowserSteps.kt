package features.sharedSteps

import net.serenitybdd.core.SerenitySystemProperties
import net.serenitybdd.core.exceptions.SerenityManagedException
import net.thucydides.core.ThucydidesSystemProperty
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.openqa.selenium.support.ui.WebDriverWait
import pages.LoginPage
import java.net.MalformedURLException
import java.net.URL
import java.time.Duration
import java.util.*

open class BrowserSteps {

    lateinit var loginPage: LoginPage

    @Step
    open fun goToApp() {
        if (!loginPage.onMobile()) {
            loginPage.open()
        }
    }

    @Step
    open fun browseTo(url: String) {
        loginPage.driver.get(url)
    }

    @Step
    open fun shouldHaveUrl(url: String) {
        WebDriverWait(loginPage.driver, 1000)
                .pollingEvery(Duration.ofMillis(100))
                .until {
                    it.currentUrl == url
                }
    }

    @Step()
    fun checkLoginDetailsAreReset() {
        val vuexCookieName = "nhso";
        var cookieValue = getCookieJson(vuexCookieName)
        val targetObject = CookieWrapper.fromJson(cookieValue)
        targetObject.assertIsLoggedOut()
    }

    @Step
    private fun getCookieJson(cookieName:String):String
    {
        var cookieValue = fetchCookieContents(cookieName)
        var start = cookieValue.indexOf("{" )
        var cookieValueTrimmed = cookieValue.removeRange(0,start)
        var end = cookieValueTrimmed.lastIndexOf("}")
        return cookieValueTrimmed.removeRange(end+1, cookieValueTrimmed.lastIndex+1)
    }

    private fun fetchCookieContents(cookieName: String): String {
        val driver = loginPage.driver
        var cookieValue = driver.manage().cookies.first { x -> x.name == cookieName }.toString()
        cookieValue = cookieValue.replace("%22", "'")
        cookieValue = cookieValue.replace("%2C", ",")
        return cookieValue
    }

    @Step
    fun changeTab(url: URL) {
        val driver = loginPage.driver

        var allWindows: MutableList<String> = mutableListOf()

        for (window in loginPage.driver.windowHandles) {
            driver.switchTo().window(window)
            allWindows.add(0, driver.currentUrl)
            if (url.host == URL(driver.currentUrl).host) {
                return
            }
        }

        throw NoSuchElementException("No tab found with $url. All windows: ${allWindows.reversed()}")
    }

    @Step
    fun changeTabToApp() {
        val baseUrl: String = SerenitySystemProperties.getProperties().getValue(ThucydidesSystemProperty.WEBDRIVER_BASE_URL)
        try {
            changeTab(URL(baseUrl))
        } catch (e: MalformedURLException) {
            val message = "Malformed URL from ${ThucydidesSystemProperty.WEBDRIVER_BASE_URL}: $baseUrl"
            println("ERROR:")
            println(message)
            throw SerenityManagedException(message, e)
        }
    }

    @Step
    fun refreshPage() {
        loginPage.driver.navigate().refresh()
    }
}

