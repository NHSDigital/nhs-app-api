package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://web.local.bitraft.io:3000/messages/")
class MessagesHubPage: HybridPageObject() {

    fun assertMessagesHubPageDisplayed() {
        pageTitle.assertIsVisible()
    }

    fun assertUnreadGPIndicatorDisplayed() {
        unreadGPIndicator.assertIsVisible()
    }

    fun assertUnreadAppIndicatorDisplayed() {
        unreadAppIndicator.assertIsVisible()
    }

    fun clickOnMenuItem(menuItemId: String) {
        getElementByLocator("//*[@id='$menuItemId']")
                .click()
    }

    fun assertMenuItemNotDisplayed(menuItemId: String) {
        getElementByLocator("//*[@id='$menuItemId']").assertElementNotPresent()
    }

    private val unreadGPIndicator = getElementByLocator("//*[@id='btn_im1_messaging_unreadIndicator']")

    private val unreadAppIndicator = getElementByLocator("//*[@id='btn_appMessaging_unreadIndicator']")

    private val pageTitle = getElementByLocator("//h1[contains(text(),\"Messages\")]",
                                                "Messages Hub title")

    private fun getElementByLocator(locator: String, helpfulName: String = ""): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = locator,
                androidLocator = null,
                page = this,
                helpfulName = helpfulName)
    }
}