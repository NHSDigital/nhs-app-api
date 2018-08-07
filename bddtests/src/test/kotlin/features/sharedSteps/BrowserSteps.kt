package features.sharedSteps

import junit.framework.TestCase.assertNull
import net.serenitybdd.core.SerenitySystemProperties
import net.serenitybdd.core.exceptions.SerenityManagedException
import net.thucydides.core.ThucydidesSystemProperty
import net.thucydides.core.annotations.Step
import org.openqa.selenium.support.ui.WebDriverWait
import pages.LoginPage
import java.net.MalformedURLException
import java.net.URL
import java.time.Duration
import java.util.*
import org.openqa.selenium.Cookie

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
    open fun waitUntilSignoutCompletes() {
        WebDriverWait(loginPage.driver, 1000)
                .pollingEvery(Duration.ofMillis(100))
                .until {
                    it.currentUrl == loginPage.driver.currentUrl
                    fetchCookie("nhso.session") == null
                }
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
        assertNull(fetchCookie("nhso.session"))
    }

    private fun fetchCookie(cookieName: String): Cookie? {
        return loginPage.driver.manage().cookies.firstOrNull { x -> x.name == cookieName }
    }

    private fun cookieExists(cookieName: String): Boolean {
        val driver = loginPage.driver
        return driver.manage().cookies.any { x -> x.name == cookieName }
    }

    @Step
    fun changeTab(url: URL) {
        val driver = loginPage.driver

        val allWindows: MutableList<String> = mutableListOf()

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