package pages

const val PAGE_LEAVE_DIALOG_MESSAGE = "If you have entered any information, it will not be saved."

open class PageLeavingWarning : NativePageObject() {

    private val warningMessage = NativePageElement(
            androidLocator = "//android.widget.TextView[@text=\"$PAGE_LEAVE_DIALOG_MESSAGE\"]",
            iOSLocator = "//XCUIElementTypeStaticText[@value=\"$PAGE_LEAVE_DIALOG_MESSAGE\" and @visible='true']",
            webDesktopLocator = "//p[@data-sid='pageLeaveWarningText']",
            page = this
    )

    private val message = HybridPageElement(
            webDesktopLocator = "//p",
            page = this
    ).withNormalisedText("If you have entered any information, it will not be saved.")

    private val stayButton = NativePageElement(
            androidLocator = "//android.widget.Button[@text='Stay on this page']",
            iOSLocator = "//XCUIElementTypeButton[@label='Stay on this page']",
            webDesktopLocator = "//button[contains(text(),'Stay on this page')]",
            page = this
    )

    private val leaveButton = NativePageElement(
            androidLocator = "//android.widget.Button[@text='Leave this page']",
            iOSLocator = "//XCUIElementTypeButton[@label='Leave this page']",
            webDesktopLocator = "//a[contains(text(),'Leave this page')]",
            page = this
    )

    fun assertIsDisplayed() {
        when (onMobile()) {
            true -> warningMessage.assertIsDisplayed()
            false -> message.assertIsVisible()
        }
    }

    fun assertIsNotDisplayed() {
        when (onMobile()) {
            true -> warningMessage.assertElementNotPresent()
            false -> message.assertElementNotPresent()
        }
    }

    fun clickStay() {
        stayButton.click()
    }

    fun clickLeave() {
        leaveButton.click()
    }
}
