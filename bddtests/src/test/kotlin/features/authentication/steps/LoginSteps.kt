package features.authentication.steps

import config.Config
import models.Patient
import net.thucydides.core.annotations.Step
import org.openqa.selenium.JavascriptExecutor
import pages.loggedOut.LoginPage
import pages.loggedOut.LoginStubPage
import webdrivers.options.OptionManager
import webdrivers.options.nojs.NoJsOption

private const val SLEEP_TIME_FOR_PROMISE: Long = 6000

open class LoginSteps {

    lateinit var loginPage: LoginPage
    private lateinit var loginStubPage: LoginStubPage

    @Step
    fun using(patient: Patient) {
        loginPage.signIn()
        if(Config.instance.autoLogin != "true" && !OptionManager.instance().isEnabled(NoJsOption::class)) {
            loginStubPage.signIn(patient)
        }
    }

    @Step
    fun skipNotificationPromptCookie(cookieExists: Boolean) {
        val executor = loginPage.driver as JavascriptExecutor

        Thread.sleep(SLEEP_TIME_FOR_PROMISE)

        val isNativeApp = executor.executeScript("""
            return window.vue.${'$'}store.state.device.isNativeApp;
        """.trimIndent())

        if (!(isNativeApp as Boolean)) {
            return
        }

        executor.executeScript("""
        window.nativeAppCallbacks.notificationsSettingsStatus("");
    """.trimIndent())

        executor.executeScript("""
        window.nativeAppCallbacks.deviceNotificationPromptCookieExists($cookieExists);
    """.trimIndent())
        }
}
