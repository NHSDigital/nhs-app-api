package pages

import org.openqa.selenium.NoSuchElementException

/**
 * This constant is used for screen lock prevention on native
 * devices.
 */
const val SCREEN_LOCK_PREVENTION_INTERVAL = 30_000L

/**
 * This constant defines the wait time for the session
 * expiry modal to appear.
 */
const val WAIT_FOR_EXPIRY_MODAL_DURATION = 120_000L

/**
 * Describes the duration to which the session expiry modal
 * is displayed before automatic logout.
 */
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
            iOSAccessID = "sessionExpiryWarningGetMoreTime",
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
           true -> this.isOnPage("You'll be logged out shortly")
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
         return when (onMobile()) {
             false -> Thread.sleep(WAIT_FOR_EXPIRY_MODAL_DURATION)
             true -> {
                 scrollAndroidNativePage()
                 waitAndScrollToKeepActive()
                 scrollAndroidNativePage()
                 waitAndScrollToKeepActive()
                 scrollAndroidNativePage()
             }
         }
     }

    fun waitAndScrollToKeepActive() {
        Thread.sleep(SCREEN_LOCK_PREVENTION_INTERVAL)
        scrollAndroidNativePage()
    }

    fun waitForSessionExpiryAfterModalDisplay(){
        Thread.sleep(SESSION_EXPIRY_MODAL_DISPLAY_DURATION)
        if(onMobile()) {
            scrollAndroidNativePage()
        }
    }
}
