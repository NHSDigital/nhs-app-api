package pages.sessionexpiry

import org.openqa.selenium.NoSuchElementException
import pages.NativePageElement
import pages.NativePageObject

const val SESSION_DIALOG_DELAY = 30_000L
const val EXPIRY_INTERVAL = 60_000L

open class SessionExpiryNative : NativePageObject() {

    val header = NativePageElement(
            androidLocator = "//*[contains(@resource-id, 'sessionExpiryWarningHeader')]",
            iOSAccessID = "sessionExpiryWarningHeader",
            webDesktopLocator = "",
            webMobileLocator = "",
            page = this
    )

    private val extendSessionButton = NativePageElement(
            androidLocator = "//*[contains(@resource-id, 'extendSession')]",
            iOSAccessID = "sessionExpiryWarningGetMoreTime",
            webDesktopLocator = "",
            webMobileLocator = "",
            page = this
    )

    private val logoutButton = NativePageElement(
            androidLocator = "//*[contains(@resource-id, 'logOut')]",
            iOSAccessID = "sessionExpiryWarningLogOut",
            webDesktopLocator = "",
            webMobileLocator = "",
            page = this
    )

    fun clickExtendSession() {
        extendSessionButton.click()
    }

    fun clickLogOut() {
        logoutButton.click()
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

    fun waitForSessionExpandDialogue(){
        scrollAndroidNativePage()
        waitAndScrollToKeepActive()
        scrollAndroidNativePage()
        waitAndScrollToKeepActive()
        scrollAndroidNativePage()
    }

    fun waitAndScrollToKeepActive() {
        Thread.sleep(SESSION_DIALOG_DELAY)
        scrollAndroidNativePage()
    }

    fun waitForSessionExiryAfterDialogue(){
        Thread.sleep(EXPIRY_INTERVAL)
        scrollAndroidNativePage()
    }

}
