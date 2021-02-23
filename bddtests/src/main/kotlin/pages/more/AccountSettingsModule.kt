package pages.more

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksContent

class AccountSettingsModule(page : HybridPageObject) : LinksElement(page, content) {
    val notifications by lazy { link(notificationsLink) }

    companion object {
        private const val notificationsLink = "Notifications"
        private const val biometricsLink = "Login options"
        private const val nhsLoginLink = "NHS login"
        private var content = LinksContent(
                linkBlockTitle = "",
                containerXPath = "//div[@data-purpose='setting-section']//li")
                .addLink(biometricsLink)
                .addLink(nhsLoginLink)
                .addLink(notificationsLink)
    }
}
