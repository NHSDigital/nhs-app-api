package pages.notifications

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.sharedElements.ToggleElement

@DefaultUrl("http://web.local.bitraft.io:3000/notifications")
class NotificationsPromptPage : HybridPageObject() {

    val notificationsToggle = ToggleElement(this, "Allow notifications", "allow_notifications")
    val continueButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Continue')]",
            page = this
    )
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

    fun assertDisplayed() {
        title.waitForElement()
        ifYouShare.assertIsVisible()
        mayIncludeFeatures.assertIsVisible()
        moreInfoParagraph.assertIsVisible()
        moreInfoLink.assertIsVisible()
        notificationsToggle.assertIsVisible()
        continueButton.assertIsVisible()
    }

    private val title by lazy {
        HybridPageElement(
                webDesktopLocator = "//h1[normalize-space(text())='Manage notifications']",
                page = this,
                helpfulName = "header")
    }
}

