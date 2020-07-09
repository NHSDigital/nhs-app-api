package features.sharedSteps

import junit.framework.TestCase
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.openqa.selenium.Cookie
import org.openqa.selenium.JavascriptExecutor
import org.openqa.selenium.support.ui.WebDriverWait
import pages.loggedOut.LoginPage
import java.time.Duration

private const val SIGN_OUT_WAIT_TIME = 1000L
private const val POLLING_DURATION = 100L

open class CookieSteps {

    lateinit var loginPage: LoginPage

    @Step
    fun checkLoginDetailsAreReset() {
        TestCase.assertNull(fetchCookie("nhso.session"))
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

    private fun fetchCookie(cookieName: String): Cookie? {
        return loginPage.driver.manage().cookies.firstOrNull { x -> x.name == cookieName }
    }

    @Step
    fun clearSessionStorage(sessionKey: String) {
        val executor = loginPage.driver as JavascriptExecutor

        executor.executeScript(String.format(
                "return window.sessionStorage.removeItem('%s');", sessionKey))
    }

    @Step
    fun hasClosedCookies(sessionKey: String): String? {
        val executor = loginPage.driver as JavascriptExecutor

        return (executor.executeScript(String.format(
                "return window.sessionStorage.getItem('%s');", sessionKey))?.toString())
    }

    @Step
    fun setInstructionsCookie(seen: String) {
        val cookie = Cookie("SkipPreRegistrationPage", seen)
        loginPage.driver.manage().addCookie(cookie)
    }

    @Step
    fun verifyCookieDoesntExist(cookieName: String) {
        Assert.assertTrue(loginPage.driver.manage().getCookieNamed(cookieName) == null)
    }
}
