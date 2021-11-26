package pages.notifications

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/notifications")
class NotificationsPromptPage : HybridPageObject() {

    var weUseNotifications = getElement("//p[contains(text(), " +
            "'We use notifications to tell you when you get a new message.')]")
    var theNhsAndConnected = getElement("//p[contains(text(), " +
            "'The NHS and connected healthcare providers, like your GP surgery, " +
            "may send you messages using the NHS App.')]")
    var turnOnNotificationsForEachDevice = getElement("//p[contains(text(), " +
            "'If you want to get notifications, you need to turn them on for each device you use to access the " +
            "NHS App.')]")
    var doYouWant = getElement("//h2[contains(text(), 'Do you want to get NHS App notifications?')]")
    var ifYouShare = getElement("//p[contains(text(), " +
            "'If you share this device with other people, they may see your notifications')]")
    var moreInfoParagraph = getElement("//p[contains(text(), 'More information is available in the')]")
    var moreInfoLink = getElement("//a[contains(text(), 'NHS App privacy policy')]")

    fun assertDisplayed() {
        title.waitForElement()
        weUseNotifications.assertIsVisible()
        theNhsAndConnected.assertIsVisible()
        turnOnNotificationsForEachDevice.assertIsVisible()
        doYouWant.assertIsVisible()
        ifYouShare.assertIsVisible()
        moreInfoParagraph.assertIsVisible()
        moreInfoLink.assertIsVisible()
    }

    private val title by lazy {
        HybridPageElement(
                webDesktopLocator = "//h1[normalize-space(text())='Turn on notifications']",
                page = this,
                helpfulName = "header")
    }
}

