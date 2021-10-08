package pages

open class PageLeavingWarning : HybridPageObject() {

    private val message = HybridPageElement(
        webDesktopLocator = "//p",
        page = this
    ).withNormalisedText("If you have entered any information, it will not be saved.")

    private val stayButton = HybridPageElement(
        webDesktopLocator = "//button[contains(text(),'Stay on this page')]",
        page = this
    )

    private val leaveButton = HybridPageElement(
        webDesktopLocator = "//a[contains(text(),'Leave this page')]",
        page = this
    )

    fun assertIsDisplayed() {
        message.assertIsVisible()
    }

    fun assertIsNotDisplayed() {
        message.assertElementNotPresent()
    }

    fun clickStay() {
        stayButton.click()
    }

    fun clickLeave() {
        leaveButton.click()
    }
}
