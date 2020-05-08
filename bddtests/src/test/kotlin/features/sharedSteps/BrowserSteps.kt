package features.sharedSteps

import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.openqa.selenium.Cookie
import org.openqa.selenium.JavascriptExecutor
import org.openqa.selenium.support.ui.WebDriverWait
import pages.HybridPageObject
import pages.account.MyAccountPage
import pages.loggedOut.LoginPage
import pages.account.LoginSettingsPage
import utils.GlobalSerenityHelpers
import utils.getOrNull
import utils.set
import webdrivers.options.ChromeOptionManager
import webdrivers.options.OptionManager
import webdrivers.options.nojs.NoJsOption
import java.net.URL
import java.time.Duration
import java.util.*

private const val LOAD_URL_WAIT_TIME = 30L
private const val POLLING_DURATION = 100L
private const val TAB_COUNT_VARIABLE = "TabCount"
private const val MAX_RETRY_COUNT = 3
private const val CHROME_RECOVERY_TIME = 5000L
open class BrowserSteps {

    lateinit var loginPage: LoginPage
    lateinit var accountPage: MyAccountPage
    lateinit var loginSettingsPage: LoginSettingsPage

    @Step
    open fun goToApp() {
        goToPage {
            openPage(loginPage)
            executeScripts()
        }
    }

    private fun goToPage(page: () -> Unit) {
        var tryCount = 0
        while(tryCount< MAX_RETRY_COUNT) {
            try {
                page()
                break
            }
            catch(e: net.thucydides.core.webdriver.DriverConfigurationError){
                println("Error opening Chrome on attempt ${++tryCount} - Error was $e")
                Thread.sleep(CHROME_RECOVERY_TIME)
            }
            catch(e: org.openqa.selenium.WebDriverException) {
                println("Error opening Chrome on attempt ${++tryCount} - Error was $e")
                Thread.sleep(CHROME_RECOVERY_TIME)
            }
        }
    }

    private fun openPage(page: HybridPageObject) {
        if (!page.onMobile() && OptionManager.instance().isEnabled(NoJsOption::class)) {
            page.open()
            val optionManager = OptionManager.instance()
            optionManager.getOptions().forEach {
                ChromeOptionManager.instance.configureOption(it)
            }
        } else {
            page.open()
        }
    }

    @Step
    open fun browseTo(url: String) {
        loginPage.driver.get(url)
        executeScripts()
    }

    @Step
    fun executeScripts() {
        val scripts = GlobalSerenityHelpers.FUNCTIONS_TO_ADD_TO_WINDOW_NATIVE_APP_OBJECT.getOrNull<ArrayList<String>>()
        if (scripts != null) {
            loginPage.attachJavascriptFunctionsToNativeAppWindow(scripts)
        }
    }

    @Step
    open fun shouldHaveUrl(url: String, originalPath: String? = null) {
        val originalPathMessage= if (originalPath != null) {" Original Path : '$originalPath'"} else {""}
        WebDriverWait(loginPage.driver, LOAD_URL_WAIT_TIME)
                .pollingEvery(Duration.ofMillis(POLLING_DURATION))
                .withMessage("Expected url to be '$url', but was '${loginPage.driver.currentUrl}'"
                        + originalPathMessage)
                .until {
                    it.currentUrl == url
                }
    }

    @Step
    open fun shouldNotHaveUrl(url: String) {
        WebDriverWait(loginPage.driver, LOAD_URL_WAIT_TIME)
                .pollingEvery(Duration.ofMillis(POLLING_DURATION))
                .until {
                    !it.currentUrl.startsWith(url)
                }
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
    fun setUserAgentSource(source: String) {
        require(source == "android" || source == "ios") { "$source is not a real native source" }

        val userAgent = "Mozilla / 5.0(Linux; Android 8.0.0; SM - G930F) AppleWebKit/537.36 (KHTML, like Gecko)" +
                "Chrome/75.0.3770.101 Mobile Safari/537.36 nhsapp-$source"

        GlobalSerenityHelpers.USER_AGENT.set(userAgent)
    }

    @Step
    fun refreshPage() {
        loginPage.driver.navigate().refresh()
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

    @Step
    fun closeApp() {
        loginPage.driver.close()
    }

    @Step
    fun setBiometricType(biometricReference: String) {
        triggerWindowDispatch(
                accountPage.driver as JavascriptExecutor,
                "loginSettings/biometricSpec",
                "{ biometricTypeReference:'${biometricReference}', enabled: false}"
        )
    }

    @Step
    fun setBiometricCompletionResult(action: String, outcome: String, errorCode: String) {
        triggerWindowDispatch(
                loginSettingsPage.driver as JavascriptExecutor,
                "loginSettings/biometricCompletion",
                "{ action:'${action}', outcome:'${outcome}', errorCode:'${errorCode}' }"
        )
    }

    @Step
    fun triggerBiometricLoginError() {
        triggerWindowDispatch(
                loginPage.driver as JavascriptExecutor,
                "login/handleBiometricLoginFailure", ""
        )
    }

    private fun triggerWindowDispatch (jsExecutor: JavascriptExecutor, event: String, arg: String) {
        jsExecutor.executeScript("""
            this.window.${'$'}nuxt.${'$'}store.dispatch(
                "$event", 
                $arg
            )"""
        )
    }

}
