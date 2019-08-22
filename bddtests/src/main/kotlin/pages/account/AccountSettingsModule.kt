package pages.account

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksContent

class AccountSettingsModule(page : HybridPageObject) : LinksElement(page, content) {
    val notifications by lazy { link(notificationsLink) }

    companion object {
        private const val notificationsLink = "Notifications"
        private var content = LinksContent(linkBlockTitle = "Account settings")
                .addLink(notificationsLink)
    }
}