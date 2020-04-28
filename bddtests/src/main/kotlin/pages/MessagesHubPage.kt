package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://web.local.bitraft.io:3000/messages/")
class MessagesHubPage: HybridPageObject() {

    fun assertMessagesHubPageDisplayed() {
        pageTitle.assertIsVisible()
    }

    fun clickOnMenuItem(menuItemId: String) {
        getElementByLocator("//*[@id='$menuItemId']")
                .click()
    }

    fun assertMenuItemNotDisplayed(menuItemId: String) {
        getElementByLocator("//*[@id='$menuItemId']").assertElementNotPresent()
    }

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