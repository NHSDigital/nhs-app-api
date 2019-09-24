package pages.account

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.sharedElements.TextBlockElement
import pages.sharedElements.ToggleElement

@DefaultUrl("http://web.local.bitraft.io:3000/account/notifications")
class NotificationsSettingsPage : HybridPageObject() {

    val notificationsToggle = ToggleElement(this, "Allow notifications", "allow_notifications")

    fun assertDisplayed() {
        notificationsToggle.assertIsVisible()
        TextBlockElement.withoutHeader(this)
                .assert(
                        "You can choose whether to allow notifications on your device.",
                        "If you share this device with other people, they may be able to see your notifications.")
    }
}