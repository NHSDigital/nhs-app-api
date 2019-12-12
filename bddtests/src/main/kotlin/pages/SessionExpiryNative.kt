package pages

import config.Config
import org.openqa.selenium.NoSuchElementException
import java.time.LocalDateTime
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
            androidLocator = "//*[contains(@resource-id, 'extendSession')]",
            iOSAccessID = "sessionExpiryWarningStayLoggedIn",
            webDesktopLocator = "//button[contains(text(),'Stay logged in')]",
            page = this
    )

    private val logoutButton = NativePageElement(
            androidLocator = "//*[contains(@resource-id, 'logOut')]",
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
            super.findByAccessibilityId(elementText)
            print("found element text")
            true
        } catch (e: NoSuchElementException){
            print("not found element text")
            false
        }
    }

     fun waitForSessionExpiryModal() {
         val timeoutMillis = TimeUnit.MINUTES.toMillis(Config.instance.sessionExpiryMinutes)
         val modalDelayMillis = timeoutMillis - SESSION_EXPIRY_MODAL_DISPLAY_DURATION
         val modalDelaySeconds = TimeUnit.MILLISECONDS.toSeconds(modalDelayMillis)
         val delayUtil = LocalDateTime.now().plusSeconds(modalDelaySeconds)

         while (LocalDateTime.now().isBefore(delayUtil)) {
             Thread.sleep(SCREEN_LOCK_PREVENTION_INTERVAL)
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
