package pages.accountAndSettings

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.sharedElements.ToggleElement

@DefaultUrl("http://web.local.bitraft.io:3000/more/account-and-settings/manage-notifications")
class NotificationsSettingsPage : HybridPageObject() {

    val notificationsToggle = ToggleElement(this, "Turn on notifications on this device",
        "allow_notifications")
    var ifYouShare = getElement(
        "//p[contains(text(), " +
                "'If you share this device with other people, they may see your notifications.')]"
    )
    var theNhsAndConnected = getElement(
        "//p[contains(text(), " +
                "'The NHS and connected healthcare providers, like your GP surgery, may send you messages using" +
                " the NHS App.')]"
    )
    var moreInfoParagraph = getElement("//p[contains(text(), 'More information is available in the')]")
    var moreInfoLink = getElement("//a[contains(text(), 'NHS App privacy policy')]")
    var manageNotifications = getElement(
        "//a[contains(text(), " +
                "'Choose how notifications are shown on this device (opens your device settings)')]"
    )
    var weUseNotifications = getElement(
        "//p[contains(text(), " +
                "'We use notifications to tell you when you get a new message.')]"
    )
    var ifYouWantToGetNotifications = getElement(
        "//p[contains(text(), " +
                "'If you want to get notifications, you need to turn them on for each device you use to access" +
                " the NHS App.')]"
    )

    fun assertDisplayed() {
        title.waitForElement()
        ifYouShare.assertIsVisible()
        weUseNotifications.assertIsVisible()
        moreInfoParagraph.assertIsVisible()
        moreInfoLink.assertIsVisible()
        notificationsToggle.assertIsVisible()
        manageNotifications.assertIsVisible()
        theNhsAndConnected.assertIsVisible()
        ifYouWantToGetNotifications.assertIsVisible()
    }

    private val title by lazy {
        HybridPageElement(
            "//h1[normalize-space(text())='Manage notifications']",
            this,
            helpfulName = "header"
        )
    }
}

