package pages

import config.Config
import java.time.Duration
import java.time.LocalDateTime
import java.time.temporal.ChronoUnit

const val SCREEN_LOCK_PREVENTION_INTERVAL = 30_000L
const val SESSION_EXPIRY_MODAL_DISPLAY_DURATION = 60_000L
const val ADDITIONAL_TIME_FOR_SESSION_TO_EXPIRE = 60_000L

open class SessionExpiry : HybridPageObject() {

    private val sessionExpiryModalDisplayDuration = Duration.ofMillis(SESSION_EXPIRY_MODAL_DISPLAY_DURATION)
    private val additionalTimeForSessionToExpire = Duration.ofMillis(ADDITIONAL_TIME_FOR_SESSION_TO_EXPIRE)

    private val timeUntilSessionExpiryAfterModalDisplay =
        sessionExpiryModalDisplayDuration.plus(additionalTimeForSessionToExpire)

    private val headerWeb = HybridPageElement(
        webDesktopLocator = "//p",
        page = this
    ).withNormalisedText("For security reasons, you'll be logged out in 1 minute.")

    private val extendSessionButton = HybridPageElement(
        webDesktopLocator = "//button[contains(text(),'Stay logged in')]",
        page = this
    )

    private val logoutButton = HybridPageElement(
        webDesktopLocator = "//a[contains(text(),'Log out')]",
        page = this
    )

    fun assertIsDisplayed() {
        headerWeb.assertIsVisible()
    }

    fun assertIsNotDisplayed() {
        headerWeb.assertElementNotPresent()
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
        }
    }
}
