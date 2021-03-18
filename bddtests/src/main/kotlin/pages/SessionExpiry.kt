package pages

import config.Config
import io.appium.java_client.AppiumDriver
import io.appium.java_client.MobileElement
import utils.GlobalSerenityHelpers
import utils.getOrFail
import webdrivers.getSpecificDriver
import java.time.Duration
import java.time.LocalDateTime
import java.time.temporal.ChronoUnit

const val SCREEN_LOCK_PREVENTION_INTERVAL = 30_000L
const val SESSION_EXPIRY_MODAL_DISPLAY_DURATION = 60_000L
const val ADDITIONAL_TIME_FOR_SESSION_TO_EXPIRE = 60_000L
const val SESSION_EXPIRY_DIALOG_MESSAGE = "For security reasons, we'll log you out of the NHS App in 1 minute."

open class SessionExpiry : NativePageObject() {

    private val sessionExpiryModalDisplayDuration = Duration.ofMillis(SESSION_EXPIRY_MODAL_DISPLAY_DURATION)
    private val additionalTimeForSessionToExpire = Duration.ofMillis(ADDITIONAL_TIME_FOR_SESSION_TO_EXPIRE)

    private val timeUntilSessionExpiryAfterModalDisplay =
            sessionExpiryModalDisplayDuration.plus(additionalTimeForSessionToExpire)


    private val headerNative = NativePageElement(
            androidLocator = "//android.widget.TextView[@text=\"$SESSION_EXPIRY_DIALOG_MESSAGE\"]",
            iOSLocator = "//XCUIElementTypeStaticText[@value=\"$SESSION_EXPIRY_DIALOG_MESSAGE\" and @visible='true']",
            webDesktopLocator = "//p[@data-sid='warningDurationInformation']",
            page = this
    )

    private val headerWeb = HybridPageElement(
            webDesktopLocator = "//p",
            page = this
    ).withNormalisedText("For security reasons, you'll be logged out in 1 minute.")

    private val extendSessionButton = NativePageElement(
            androidLocator = "//android.widget.Button[@text='STAY LOGGED IN']",
            iOSLocator = "//XCUIElementTypeButton[@label='Stay logged in']",
            webDesktopLocator = "//button[contains(text(),'Stay logged in')]",
            page = this
    )

    private val logoutButton = NativePageElement(
            androidLocator = "//android.widget.Button[@text='LOG OUT']",
            iOSLocator = "//XCUIElementTypeButton[@label='Log out']",
            webDesktopLocator = "//a[contains(text(),'Log out')]",
            page = this
    )

    fun assertIsDisplayed() {
        when (onMobile()) {
            true -> headerNative.assertIsDisplayed()
            false -> headerWeb.assertIsVisible()
        }
    }

    fun assertIsNotDisplayed() {
        when (onMobile()) {
            true -> headerNative.assertElementNotPresent()
            false -> headerWeb.assertElementNotPresent()
        }
    }

    fun clickExtendSession() {
        extendSessionButton.click()
    }

    fun clickLogOut() {
        logoutButton.click()
    }

    fun waitForSessionExpiry() {
        waitForSessionExpiryModal()
        waitForSessionExpiryAfterModalDisplay()
    }

    fun waitForSessionExpiryModal(customFrom: LocalDateTime? = null) {
        val modalDelay = timeUntilSessionExpiryModalShouldBeDisplayed()
        waitFor(modalDelay, customFrom)
    }

    fun waitForSessionExpiryAfterModalDisplay() {
        waitFor(timeUntilSessionExpiryAfterModalDisplay)
    }

    fun backgroundAppUntilSessionExpiryModalShouldBeDisplayed() {
        val modalDelay = timeUntilSessionExpiryModalShouldBeDisplayed()
        backgroundAppFor(modalDelay)
    }

    fun backgroundAppUntilSessionExpiry() {
        val expiryDelay = timeUntilSessionExpiry()
        backgroundAppFor(expiryDelay)
    }

    private fun backgroundAppFor(delay: Duration) {
        val driver = driver.getSpecificDriver<AppiumDriver<MobileElement>>()
        driver.runAppInBackground(Duration.ofSeconds(-1))
        waitFor(delay)
        driver.activateApp(GlobalSerenityHelpers.APP_BUNDLE_ID.getOrFail())
    }

    private fun timeUntilSessionExpiry(): Duration =
            timeUntilSessionExpiryModalShouldBeDisplayed() + timeUntilSessionExpiryAfterModalDisplay

    private fun timeUntilSessionExpiryModalShouldBeDisplayed(): Duration {
        val timeoutDuration = Duration.ofMinutes(Config.instance.sessionExpiryMinutes)
        return timeoutDuration.minus(sessionExpiryModalDisplayDuration)
    }

    private fun waitFor(modalDelay: Duration, customFrom: LocalDateTime? = null) {
        val from = customFrom ?: LocalDateTime.now()
        val delayUntil = from.plus(modalDelay)

        while (true) {
            val now = LocalDateTime.now()
            val delay = now
                    .until(delayUntil, ChronoUnit.MILLIS)
                    .coerceAtMost(SCREEN_LOCK_PREVENTION_INTERVAL)
            if (delay <= 0) {
                break
            }

            println("$now: Sleeping ${delay}ms until $delayUntil")
            Thread.sleep(delay)

            tryUnlockDevice()
        }
    }
}
