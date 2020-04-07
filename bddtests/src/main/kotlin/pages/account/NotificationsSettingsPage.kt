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
                .paragraph(
                        "If you share this device with other people, they may be able to see your notifications.")
                .toggle("Allow notifications")
                .span("Manage how notifications appear on your device")
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