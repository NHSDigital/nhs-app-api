package pages

import config.Config
import org.openqa.selenium.NoSuchElementException
import java.time.LocalDateTime
import java.time.temporal.ChronoUnit
import java.util.concurrent.TimeUnit

const val SCREEN_LOCK_PREVENTION_INTERVAL = 30_000L
const val SESSION_EXPIRY_MODAL_DISPLAY_DURATION = 60_000L

open class SessionExpiryNative : NativePageObject() {

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

    fun isOnPage(elementText: String): Boolean {
        return try {
            findNativeByXpath("//android.widget.TextView[@text = \"$elementText\"]")
            println("found element text: $elementText")
            true
        } catch (e: NoSuchElementException){
            println("not found element text: $elementText")
            false
        }
    }

     fun waitForSessionExpiryModal() {
         val timeoutMillis = TimeUnit.MINUTES.toMillis(Config.instance.sessionExpiryMinutes)
         val modalDelayMillis = timeoutMillis - SESSION_EXPIRY_MODAL_DISPLAY_DURATION
         val modalDelaySeconds = TimeUnit.MILLISECONDS.toSeconds(modalDelayMillis)
         val delayUntil = LocalDateTime.now().plusSeconds(modalDelaySeconds)

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

    fun waitForSessionExpiryAfterModalDisplay(){
        Thread.sleep(SESSION_EXPIRY_MODAL_DISPLAY_DURATION)
        if(onMobile()) {
            scrollAndroidNativePage()
        }
    }
}
