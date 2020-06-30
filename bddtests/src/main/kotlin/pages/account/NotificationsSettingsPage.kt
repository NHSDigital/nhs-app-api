package pages.account

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.sharedElements.expectedPage.ExpectedPageStructure
import pages.sharedElements.ToggleElement

@DefaultUrl("http://web.local.bitraft.io:3000/account/notifications")
class NotificationsSettingsPage : HybridPageObject() {

    val notificationsToggle = ToggleElement(this, "Allow notifications", "allow_notifications")

    fun assertDisplayed() {
        title.waitForElement()
        val expected = ExpectedPageStructure()
                .paragraph("You can choose whether to allow notifications on your device.")
                .paragraph("If you share this device with other people, they may be able to see your " +
                        "notifications. Read more about notifications in the NHS App privacy policy.")
                .toggle("Allow notifications", "I accept the NHS App sending me notifications on this device")
                .paragraph("Manage how notifications are shown on this device (opens your device settings)")
        expected.assert(this)
    }

    private val title by lazy {
        HybridPageElement(
                "//h1[normalize-space(text())='Manage notifications']",
                "//h1[normalize-space(text())='Manage notifications']",
                null,
                null,
                this,
                helpfulName = "header")
    }
}