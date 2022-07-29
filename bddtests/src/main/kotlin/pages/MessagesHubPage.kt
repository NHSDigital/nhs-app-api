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
    
    fun assertUnreadAppCountDisplayed() {
        unreadAppCount.assertIsVisible()
    }

    fun clickOnMenuItem(menuItemId: String) {
        getElementByLocator("//*[@id='$menuItemId']")
                .click()
    }

    fun assertMenuItemNotDisplayed(menuItemId: String) {
        getElementByLocator("//*[@id='$menuItemId']").assertElementNotPresent()
    }

    private val unreadGPIndicator = getElementByLocator("//*[@id='btn_im1_messaging_discIndicator']")
    
    private val unreadAppCount = getElementByLocator("//*[@id='btn_appMessaging_countIndicator']")

    private val pageTitle = getElementByLocator("//h1[contains(text(),\"Messages\")]",
                                                "Messages Hub title")

    private fun getElementByLocator(locator: String, helpfulName: String = ""): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator = locator,
            page = this,
            helpfulName = helpfulName)
    }
}
