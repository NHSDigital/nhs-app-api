package pages.notifications

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject

@DefaultUrl("http://web.local.bitraft.io:3000/notifications-failure")
class NotificationsPromptFailurePage : HybridPageObject() {
    val continueButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Continue')]",
            page = this
    )

    fun assertDisplayed() {
        title.waitForElement()
    }

    private val title by lazy {
        HybridPageElement(
                webDesktopLocator = "//h1[normalize-space(text())='Sorry, we could not set your notifications choice']",
                page = this,
                helpfulName = "header")
    }
}

