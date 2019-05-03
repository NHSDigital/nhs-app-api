package features.sharedSteps

import junit.framework.TestCase.assertNull
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.openqa.selenium.Cookie
import org.openqa.selenium.support.ui.WebDriverWait
import pages.loggedOut.LoginPage
import webdrivers.options.ChromeOptionManager
import webdrivers.options.OptionManager
import webdrivers.options.nojs.NoJsOption
import java.net.URL
import java.time.Duration
import java.util.*

private const val SIGN_OUT_WAIT_TIME = 1000L
private const val LOAD_URL_WAIT_TIME = 30L
private const val POLLING_DURATION = 100L
private const val TAB_COUNT_VARIABLE = "TabCount"

open class BrowserSteps {

    lateinit var loginPage: LoginPage

    @Step
    open fun goToApp() {
        if (!loginPage.onMobile() && OptionManager.instance().isEnabled(NoJsOption::class)) {
            loginPage.open()
            val optionManager = OptionManager.instance()
            optionManager.getOptions().forEach {
                ChromeOptionManager.instance.configureOption(it)
            }
        }
        else {
                loginPage.open()
            }
    }

    @Step
    open fun browseTo(url: String) {
        loginPage.driver.get(url)
    }

    @Step
    open fun waitUntilSignoutCompletes() {
        WebDriverWait(loginPage.driver, SIGN_OUT_WAIT_TIME)
                .pollingEvery(Duration.ofMillis(POLLING_DURATION))
                .until {
                    it.currentUrl == loginPage.driver.currentUrl ||
                            fetchCookie("nhso.session") == null
                }
    }

    @Step
    open fun shouldHaveUrl(url: String) {
        WebDriverWait(loginPage.driver, LOAD_URL_WAIT_TIME)
                .pollingEvery(Duration.ofMillis(POLLING_DURATION))
                .until {
                    it.currentUrl.startsWith(url)
                }
    }

    @Step
    fun checkLoginDetailsAreReset() {
        assertNull(fetchCookie("nhso.session"))
    }

    private fun fetchCookie(cookieName: String): Cookie? {
        return loginPage.driver.manage().cookies.firstOrNull { x -> x.name == cookieName }
    }

    @Step
    fun cookieExists(cookieName: String): Boolean {
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
            val currentTabUrl = URL(driver.currentUrl)
            if(url.host == currentTabUrl.host)
                return
        }

        throw NoSuchElementException("No tab found with $url. All windows: ${allWindows.reversed()}")
    }

    @Step
    fun storeCurrentTabCount() {
        Serenity.setSessionVariable(TAB_COUNT_VARIABLE).to(loginPage.driver.windowHandles.count())
    }

    @Step
    fun assertNewTab() {
        val handles = loginPage.driver.windowHandles
        Assert.assertTrue("Expected a new tab to be opened",
                handles.count()==Serenity.sessionVariableCalled<Int>(TAB_COUNT_VARIABLE) + 1)
    }

    @Step
    fun appendSourceQueryString(source: String) {
        val driver = loginPage.driver
        var url = driver.currentUrl

        url += "?source=$source"

        browseTo(url)
    }
}
