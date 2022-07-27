package pages.accountAndSettings

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.sharedElements.ToggleElement

@DefaultUrl("http://web.local.bitraft.io:3000/more/account-and-settings/manage-notifications")
class NotificationsSettingsPage : HybridPageObject() {

    private val listMenuPath = "//ul[@data-purpose='manage-notifications-menu']//li//a/div/h2"

    private fun link(linkText: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$listMenuPath${String.format(containsTextXpathSubstring, linkText)}",
                page = this,
                helpfulName = "$linkText Link")
    }

    val notificationsToggle = ToggleElement(this, "Tell me when I get new messages from my GP surgery" +
            " and other healthcare services",
        "allow_notifications")

    var notificationMottoHelpTextStart = getElement(
        "//p[contains(text(),'Turn on notifications to:')]")

    var moreInfoParagraph = getElement("//p[contains(text(), 'More information is available in the')]")
    var moreInfoLink = getElement("//a[contains(text(), 'NHS account privacy policy')]")
    var manageNotifications = getElement(
        "//a[contains(text(), " +
                "'Choose how notifications are shown on this device (opens your device settings)')]")
    var exampleNotifications = link("See example notifications")
    var moreThanOneDevice =  link("Managing notifications if you have more than one device")

    fun assertDisplayed() {
        title.waitForElement()
        moreInfoParagraph.assertIsVisible()
        moreInfoLink.assertIsVisible()
        notificationsToggle.assertIsVisible()
        manageNotifications.assertIsVisible()
        notificationMottoHelpTextStart.assertIsVisible()
        exampleNotifications.assertIsVisible()
        moreThanOneDevice.assertIsVisible()
    }

    private val title by lazy {
        HybridPageElement(
            "//h1[normalize-space(text())='Manage notifications']",
            this,
            helpfulName = "header"
        )
    }
}

