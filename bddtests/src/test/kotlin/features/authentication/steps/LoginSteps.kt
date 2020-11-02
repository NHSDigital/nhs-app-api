package features.authentication.steps

import config.Config
import features.pushNotifications.stepDefinitions.NotificationsFactory
import features.pushNotifications.stepDefinitions.PushNotificationsSerenityHelpers
import features.pushNotifications.stepDefinitions.SettingStatus
import features.sharedSteps.BrowserSteps
import models.Patient
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import org.openqa.selenium.JavascriptExecutor
import pages.loggedOut.LoginPage
import pages.loggedOut.LoginStubPage
import utils.GlobalSerenityHelpers
import utils.clearList
import utils.getOrNull
import webdrivers.options.OptionManager
import webdrivers.options.nojs.NoJsOption
import java.time.LocalDateTime

private const val TIMEOUT_FOR_PROMISE: Long = 10000
private const val SLEEP_TIME_FOR_PROMISE: Long = 6000
private const val WAIT_INCREMENT: Long = 10

open class LoginSteps {

    lateinit var loginPage: LoginPage
    private lateinit var loginStubPage: LoginStubPage

    @Steps
    lateinit var browser: BrowserSteps

    @Step
    fun using(patient: Patient) {
        stubbedLoginAndResetScripts(
                patient,
                settingStatus = SettingStatus.NotDetermined,
                authorised = true,
                cookieExists = true)
    }

    @Step
    fun usingLoginWithNotificationOptions(
            patient: Patient,
            settingStatus: SettingStatus,
            authorised: Boolean,
            cookieExists: Boolean ) {
        stubbedLoginAndResetScripts(patient, settingStatus, authorised, cookieExists)
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
            settingStatus: SettingStatus,
            authorised: Boolean,
            cookieExists: Boolean
    ) {
        loginPage.signIn()
        if(Config.instance.autoLogin != "true" && !OptionManager.instance().isEnabled(NoJsOption::class)) {
            loginStubPage.signIn(patient)

            val userAgent = GlobalSerenityHelpers.USER_AGENT.getOrNull<String>()

            if (!userAgent.isNullOrBlank()) {
                val timeout = LocalDateTime.now().plusSeconds(TIMEOUT_FOR_PROMISE)

                while (
                        loginPage.driver.currentUrl.startsWith(
                                "http://auth.nhslogin.stubs.local.bitraft.io")
                        && LocalDateTime.now()!! < timeout) {
                    Thread.sleep(WAIT_INCREMENT)
                }

                resetScripts(settingStatus, authorised, cookieExists)
            }
        }
    }

    private fun resetScripts(status: SettingStatus, authorised: Boolean, cookieExists: Boolean) {
        GlobalSerenityHelpers.FUNCTIONS_TO_ADD_TO_WINDOW_NATIVE_APP_OBJECT.clearList<String>()

        val factory = NotificationsFactory()

        if (PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrNull<String>().isNullOrBlank()) {
            val patient = GlobalSerenityHelpers.PATIENT.getOrNull<Patient>()

            if (patient === null) {
                factory.setUpUser()
            } else {
                factory.setUpUser(patient = patient)
            }

            factory.setUpDeviceValues()
        }

        factory.mockNativeNotificationFunctions(status, authorised, cookieExists)

        if (status === SettingStatus.Authorised && authorised) {
            factory.setUpExistingRegistration()
        }

        browser.executeScripts()
    }
}
