package pages

import config.Config
import io.appium.java_client.AppiumDriver
import io.appium.java_client.MobileElement
import org.openqa.selenium.NoSuchElementException
import webdrivers.getSpecificDriver
import java.time.Duration
import java.time.LocalDateTime
import java.time.temporal.ChronoUnit

const val SCREEN_LOCK_PREVENTION_INTERVAL = 30_000L
const val SESSION_EXPIRY_MODAL_DISPLAY_DURATION = 60_000L
const val ADDITIONAL_TIME_FOR_SESSION_TO_EXPIRE = 60_000L

open class SessionExpiryNative : NativePageObject() {

    private val sessionExpiryModalDisplayDuration = Duration.ofMillis(SESSION_EXPIRY_MODAL_DISPLAY_DURATION)
    private val additionalTimeForSessionToExpire = Duration.ofMillis(ADDITIONAL_TIME_FOR_SESSION_TO_EXPIRE)

    private val timeUntilSessionExpiryAfterModalDisplay =
            sessionExpiryModalDisplayDuration.plus(additionalTimeForSessionToExpire)

    val header = NativePageElement(
            androidLocator = "//*[contains(@resource-id, 'sessionExpiryWarningHeader')]",
            iOSAccessID = "sessionExpiryWarningHeader",
            webDesktopLocator = "//p[@data-sid='warningDurationInformation']",
            page = this
    )

    private val extendSessionButton = NativePageElement(
            androidLocator = "//android.widget.Button[@text = 'STAY LOGGED IN']",
            iOSAccessID = "sessionExpiryWarningStayLoggedIn",
            webDesktopLocator = "//button[contains(text(),'Stay logged in')]",
            page = this
    )

    private val logoutButton = NativePageElement(
            androidLocator = "//android.widget.Button[@text = 'LOG OUT']",
            iOSAccessID = "sessionExpiryWarningLogOut",
            webDesktopLocator = "//a[contains(text(),'Log out')]",
            page = this
    )

     fun clickExtendSession() {
        extendSessionButton.click()
    }

     fun clickLogOut() {
        logoutButton.click()
    }

     fun isSessionExpiryModalVisible() : Boolean {
        return when(onMobile()) {
           true -> this.isOnPage("For security reasons, we'll log you out of the NHS App in 1 minute.")
           false -> header.withoutRetrying().elements.count() > 0
        }
    }

    private fun isOnPage(elementText: String): Boolean {
        return try {
            findNativeByXpath("//android.widget.TextView[@text = \"$elementText\"]")
            println("found element text: $elementText")
            true
        } catch (e: NoSuchElementException){
            println("not found element text: $elementText")
            false
        }
    }

    fun waitForSessionExpiry() {
        waitForSessionExpiryModal()
        waitForSessionExpiryAfterModalDisplay()
    }

    fun waitForSessionExpiryModal() {
        val modalDelay = timeUntilSessionExpiryModalShouldBeDisplayed()
        waitFor(modalDelay)
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
        driver.runAppInBackground(delay)
        scrollAndroidNativePage()
    }

    private fun timeUntilSessionExpiry(): Duration =
            timeUntilSessionExpiryModalShouldBeDisplayed() + timeUntilSessionExpiryAfterModalDisplay

    private fun timeUntilSessionExpiryModalShouldBeDisplayed(): Duration {
        val timeoutDuration = Duration.ofMinutes(Config.instance.sessionExpiryMinutes)
        return timeoutDuration.minus(sessionExpiryModalDisplayDuration)
    }

    private fun waitFor(modalDelay: Duration) {
        val delayUntil = LocalDateTime.now().plus(modalDelay)

        while (true) {
            val now = LocalDateTime.now()
            val delay = now
                    .until(delayUntil, ChronoUnit.MILLIS)
                    .coerceAtMost(SCREEN_LOCK_PREVENTION_INTERVAL)
            if (delay <= 0) {
                break;
            }
            println("$now: Sleeping ${delay}ms until $delayUntil")
            Thread.sleep(delay)
            if (onMobile()) {
                scrollAndroidNativePage()
            }
        }
    }
}
