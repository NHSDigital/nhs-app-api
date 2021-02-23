package pages.more

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.sharedElements.ToggleElement

@DefaultUrl("http://web.local.bitraft.io:3000/more/notifications")
class NotificationsSettingsPage : HybridPageObject() {

    val notificationsToggle = ToggleElement(this, "Allow notifications", "allow_notifications")
    var ifYouShare =
            getElement(
                    "//p[contains(text(), " +
                            "'If you share this device with other people, they may see your notifications. " +
                            "The settings will apply to everyone who logs in to the NHS App on this device.')]")
    var mayIncludeFeatures =
            getElement("//p[contains(text(), " +
                    "'These may include new features and public health updates.')]")
    var moreInfoParagraph =
            getElement("//p[contains(text(), 'More information is available in the')]")
    var moreInfoLink =
            getElement("//a[contains(text(), 'NHS App privacy policy')]")
    var manageNotifications =
            getElement(
                    "//a[contains(text(), " +
                            "'Manage how notifications are shown on this device (opens your device settings)')]")

    fun assertDisplayed() {
        title.waitForElement()
        ifYouShare.assertIsVisible()
        mayIncludeFeatures.assertIsVisible()
        moreInfoParagraph.assertIsVisible()
        moreInfoLink.assertIsVisible()
        notificationsToggle.assertIsVisible()
        manageNotifications.assertIsVisible()
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
