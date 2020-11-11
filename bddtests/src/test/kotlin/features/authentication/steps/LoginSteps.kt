package features.authentication.steps

import config.Config
import features.sharedSteps.BrowserSteps
import models.Patient
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import org.openqa.selenium.Cookie
import org.openqa.selenium.JavascriptExecutor
import pages.loggedOut.LoginPage
import pages.loggedOut.LoginStubPage
import utils.GlobalSerenityHelpers
import utils.getOrNull
import webdrivers.options.OptionManager
import webdrivers.options.nojs.NoJsOption

private const val SLEEP_TIME_FOR_PROMISE: Long = 6000

open class LoginSteps {

    lateinit var loginPage: LoginPage
    private lateinit var loginStubPage: LoginStubPage

    @Steps
    lateinit var browser: BrowserSteps

    @Step
    fun using(patient: Patient) {
        stubbedLoginAndResetScripts(
                patient, true)

        val userAgent = GlobalSerenityHelpers.USER_AGENT.getOrNull<String>()

        if (!userAgent.isNullOrBlank()) {
            val executor = loginPage.driver as JavascriptExecutor

            Thread.sleep(SLEEP_TIME_FOR_PROMISE)

            executor.executeScript("""
        window.nativeAppCallbacks.notificationsSettingsStatus("");
    """.trimIndent())
        }
    }

    @Step
    fun usingLoginWithNotificationOptions(
            patient: Patient) {
        stubbedLoginAndResetScripts(patient, false)
        val executor = loginPage.driver as JavascriptExecutor

        Thread.sleep(SLEEP_TIME_FOR_PROMISE)

        executor.executeScript("""
        window.nativeAppCallbacks.notificationsSettingsStatus("");
    """.trimIndent())
    }

    @Step
    fun skipNotificationPromptCookie() {
        val executor = loginPage.driver as JavascriptExecutor

        Thread.sleep(SLEEP_TIME_FOR_PROMISE)

        executor.executeScript("""
        window.nativeAppCallbacks.notificationsSettingsStatus("");
    """.trimIndent())
    }

    private fun stubbedLoginAndResetScripts (
            patient: Patient,
            shouldAddNotificationsCookie: Boolean
    ) {
        loginPage.signIn()
        if(Config.instance.autoLogin != "true" && !OptionManager.instance().isEnabled(NoJsOption::class)) {
            loginStubPage.signIn(patient)

            val userAgent = GlobalSerenityHelpers.USER_AGENT.getOrNull<String>()

            if (!userAgent.isNullOrBlank() && shouldAddNotificationsCookie) {
                val cookie = Cookie("nhso.notifications-prompt-${patient.subject}", patient.subject)
                loginPage.driver.manage().addCookie(cookie)
            }
        }
    }
}
